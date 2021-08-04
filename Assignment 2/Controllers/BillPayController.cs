using MvcMCBA.Models;
using MvcMCBA.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using MvcMCBA.Authorise;

namespace MvcMCBA.Controllers
{
    [SecureContent]
    public class BillPayController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public BillPayController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> List(int? page = 1)
        {
            var response = await Client.GetAsync($"api/Customer/{HttpContext.Session.GetInt32(nameof(Customer.CustomerID))}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            var accountNumber = new List<int>();
            foreach (var a in customer.Accounts)
            {
                accountNumber.Add(a.AccountNumber);
            }

            response = await Client.GetAsync("api/BillPay");
            result = await response.Content.ReadAsStringAsync();
            var billpay = JsonConvert.DeserializeObject<List<BillPay>>(result);

            var accountBillPays = new List<BillPay>();
            foreach (var b in billpay)
            {
                if (accountNumber.Contains(b.AccountNumber))
                    accountBillPays.Add(b);
            }
            // Page the orders, maximum of X per page.
            const int pageSize = 4;
            var pagedList = accountBillPays.OrderBy(x => x.ScheduleTimeUtc).ToPagedList((int)page, pageSize);
            return View(pagedList);
        }

        public async Task<IActionResult> NewPayment(int payeeid)
        {
            var response = await Client.GetAsync($"api/Customer/{HttpContext.Session.GetInt32(nameof(Customer.CustomerID))}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            var accountNumber = new List<SelectListItem>();
            foreach (var a in customer.Accounts)
            {
                accountNumber.Add(new SelectListItem { Value = a.AccountNumber.ToString(), Text = a.AccountNumber.ToString() });
            }
            BillPayViewModel billpayModel = new()
            {
                PayeeID = payeeid,
                AccountNumberList = accountNumber
            };
            return View(billpayModel);
        }
        [HttpPost]
        public async Task<IActionResult> NewPayment(BillPayViewModel viewModel)
        {
            var response = await Client.GetAsync($"api/Customer/{HttpContext.Session.GetInt32(nameof(Customer.CustomerID))}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            bool ownAccount = false;
            foreach (var a in customer.Accounts)
                if (a.AccountNumber == Int32.Parse(viewModel.AccountNumber))
                    ownAccount = true;
            if (!ownAccount)
                ModelState.AddModelError(nameof(viewModel.AccountNumber), "Enter one of your Accounts");
            if (DateTime.Compare(viewModel.ScheduleTimeUtc, DateTime.Now) < 0)
                ModelState.AddModelError(nameof(viewModel.ScheduleTimeUtc), "Set a future time");
            if (!viewModel.Amount.ToString().IsDollarAmount())
                ModelState.AddModelError(nameof(viewModel.Amount), "Enter a dollar amount");

            if (!ModelState.IsValid)
            {
                response = await Client.GetAsync($"api/Customer/{HttpContext.Session.GetInt32(nameof(Customer.CustomerID))}");
                if (!response.IsSuccessStatusCode)
                    throw new Exception();
                result = await response.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<Customer>(result);

                var accountNumber = new List<SelectListItem>();
                foreach (var a in customer.Accounts)
                {
                    accountNumber.Add(new SelectListItem { Value = a.AccountNumber.ToString(), Text = a.AccountNumber.ToString() });
                }
                viewModel.AccountNumberList = accountNumber;
                return View(viewModel);
            }

            BillPay billpay = new()
            {
                AccountNumber = Int32.Parse(viewModel.AccountNumber),
                PayeeID = viewModel.PayeeID,
                Amount = viewModel.Amount,
                ScheduleTimeUtc = viewModel.ScheduleTimeUtc.ToUniversalTime(),
                Period = Enum.Parse<PaymentPeriod>(viewModel.Period),
                Status = BillPayStatus.Ready,
            };

            var content = new StringContent(JsonConvert.SerializeObject(billpay), Encoding.UTF8, "application/json");
            response = Client.PostAsync("api/BillPay", content).Result;
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(List));
            //return View(viewModel);
            throw new Exception(billpay.PayeeID.ToString());
        }
        public async Task<IActionResult> Cancel(int billpayid)
        {
            var response = await Client.GetAsync($"api/BillPay/{billpayid}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var billpay = JsonConvert.DeserializeObject<BillPay>(result);
            return View(billpay);
        }

        [HttpPost, ActionName("Cancel")]
        public IActionResult CancelConfirmed(int billpayid)
        {
            var response = Client.DeleteAsync($"api/BillPay/{billpayid}").Result;
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(List));
            return NotFound();
        }

        public async Task<IActionResult> Edit(int billpayid)
        {
            var response = await Client.GetAsync($"api/BillPay/{billpayid}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var billpay = JsonConvert.DeserializeObject<BillPay>(result);
            return View(billpay);
        }

        [HttpPost]
        public IActionResult Edit(BillPay billpay)
        {
            if (ModelState.IsValid)
            {
                billpay.Status = BillPayStatus.Ready;
                var content = new StringContent(JsonConvert.SerializeObject(billpay), Encoding.UTF8, "application/json");
                var response = Client.PutAsync("api/BillPay", content).Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(List));
            }
            return View(billpay);
        }
    }
}
