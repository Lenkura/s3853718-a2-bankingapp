using MvcMCBA.Authorise;
using MvcMCBA.Data;
using MvcMCBA.Models;
using MvcMCBA.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using System.Net.Http;

namespace MvcMCBA.Controllers
{
    public class StatementController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public StatementController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index()
        {
           // var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
            var id = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var response = await Client.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);
            return View(customer);
        }

        /*   public async Task<IActionResult> History(int accountNumber)
           {
               var account = await _context.Accounts.FindAsync(accountNumber);
               var transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();
               return View(new StatementViewModel
               {
                   AccountNumber = accountNumber,
                   PageNumber = 0,
                   Transactions = transactions,
               });
           }
           [HttpPost]
           public async Task<IActionResult> History(StatementViewModel viewmodel)
           {
               var account = await _context.Accounts.FindAsync(viewmodel.AccountNumber);
               var transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();
               return View(new StatementViewModel
               {
                   AccountNumber = viewmodel.AccountNumber,
                   PageNumber = viewmodel.PageNumber,
                   Transactions = transactions,
               });
           }*/

        public IActionResult IndexingHistory(int accountNumber)
        {
            HttpContext.Session.SetInt32("AccountHistory", accountNumber);
            return RedirectToAction(nameof(History));
        }
        public async Task<IActionResult> History(int? page = 1)
        {
            var id = HttpContext.Session.GetInt32("AccountHistory");
            var response = await Client.GetAsync($"api/Account/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var account = JsonConvert.DeserializeObject<Account>(result);
            ViewBag.Account = account;

            response = await Client.GetAsync($"api/Transaction/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            result = await response.Content.ReadAsStringAsync();
            var accountTransaction = JsonConvert.DeserializeObject<List<Transaction>>(result);

            // Page the orders, maximum of X per page.
            const int pageSize = 4;
            var pagedList = accountTransaction.ToPagedList((int)page, pageSize);

            //var pagedList = await _context.Transactions.Where(x => x.AccountNumber == account.AccountNumber).
            //  OrderByDescending(x => x.TransactionTimeUtc).ToPagedListAsync(page, pageSize);

            return View(pagedList);
        }

    }
}
