using MvcMCBA.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using DataValidator;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MvcMCBA.Background_Services
{
    public class BillPayBackgroundService : BackgroundService
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public BillPayBackgroundService(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PayScheduledBills(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        private async Task PayScheduledBills(CancellationToken stoppingToken)
        {
            var response = await Client.GetAsync("api/BillPay", stoppingToken);
            var result = await response.Content.ReadAsStringAsync(stoppingToken);
            var billpays = JsonConvert.DeserializeObject<List<BillPay>>(result);

            foreach (var bill in billpays)
            {
                if (DateTime.Compare(bill.ScheduleTimeUtc, DateTime.Now) < 0 && bill.Status.Equals(BillPayStatus.R))
                {
                    response = await Client.GetAsync($"api/Account/{bill.AccountNumber}", stoppingToken);
                    if (!response.IsSuccessStatusCode)
                        break;
                    result = await response.Content.ReadAsStringAsync(stoppingToken);
                    var account = JsonConvert.DeserializeObject<Account>(result);

                    if (account.Balance - bill.Amount < AccountChecks.GetAccountTypeMin(account.AccountType.ToString()))
                    {
                        bill.Status = BillPayStatus.E;
                        var content = new StringContent(JsonConvert.SerializeObject(bill), Encoding.UTF8, "application/json");
                        response = Client.PutAsync("api/BillPay", content, stoppingToken).Result;
                        if (response.IsSuccessStatusCode)
                            break;
                    }
                    else
                    {

                        account.Balance -= bill.Amount;
                        var content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");
                        response = Client.PutAsync("api/Account", content, stoppingToken).Result;

                        var billpayTransaction = new Transaction
                        {
                            AccountNumber = account.AccountNumber,
                            TransactionType = TransactionType.B,
                            Amount = bill.Amount,
                            TransactionTimeUtc = DateTime.UtcNow
                        };
                        content = new StringContent(JsonConvert.SerializeObject(billpayTransaction), Encoding.UTF8, "application/json");
                        response = Client.PostAsync("api/Transaction", content, stoppingToken).Result;

                        switch (bill.Period)
                        {
                            case PaymentPeriod.O:
                                response = Client.DeleteAsync($"api/Billpay/{bill.BillPayID}", stoppingToken).Result;
                                break;
                            case PaymentPeriod.M:
                                bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1);
                                content = new StringContent(JsonConvert.SerializeObject(bill), Encoding.UTF8, "application/json");
                                response = Client.PutAsync("api/BillPay", content, stoppingToken).Result;
                                break;
                            case PaymentPeriod.Q:
                                bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(3);
                                content = new StringContent(JsonConvert.SerializeObject(bill), Encoding.UTF8, "application/json");
                                response = Client.PutAsync("api/BillPay", content, stoppingToken).Result;
                                break;
                            case PaymentPeriod.Y:
                                bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddYears(1);
                                content = new StringContent(JsonConvert.SerializeObject(bill), Encoding.UTF8, "application/json");
                                response = Client.PutAsync("api/BillPay", content, stoppingToken).Result;
                                break;

                        }
                    }
                }

            }
        }
    }
}
