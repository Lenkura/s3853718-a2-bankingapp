using Assignment_2.Data;
using Assignment_2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using DataValidator;

namespace Assignment_2.Background_Services
{
    public class BillPayBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        public BillPayBackgroundService(IServiceProvider services)
        {
            _services = services;
        }
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
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Assignment2DbContext>();

            var billpays = await context.BillPays.ToListAsync(stoppingToken);
            foreach (var bill in billpays)
            {
                if (DateTime.Compare(bill.ScheduleTimeUtc, DateTime.Now) < 0 && bill.Status.Equals(BillPayStatus.Ready))
                {
                    var account = await context.Accounts.FindAsync(bill.AccountNumber);
                    if (account.Balance - bill.Amount < AccountChecks.GetAccountTypeMin(account.AccountType.ToString()))
                    {
                        bill.Status = BillPayStatus.Error;
                        context.BillPays.Update(bill);
                    }
                    else
                    {

                        account.Balance -= bill.Amount;
                        account.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = TransactionType.B,
                            Amount = bill.Amount,
                            TransactionTimeUtc = DateTime.UtcNow
                        });


                        switch (bill.Period)
                        {
                            case PaymentPeriod.O:
                                context.BillPays.Remove(bill);
                                break;
                            case PaymentPeriod.M:
                                bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1);
                                context.BillPays.Update(bill);
                                break;
                            case PaymentPeriod.Q:
                                bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(3);
                                context.BillPays.Update(bill);
                                break;
                            case PaymentPeriod.Y:
                                bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddYears(1);
                                context.BillPays.Update(bill);
                                break;

                        }
                    }
                }

            }
            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
