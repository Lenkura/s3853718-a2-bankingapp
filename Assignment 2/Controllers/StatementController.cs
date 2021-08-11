using MvcMCBA.Authorise;
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
using MvcMCBA.Data;
using Microsoft.AspNetCore.Authorization;

namespace MvcMCBA.Controllers
{
    [Authorize(Roles = "Customer")]
    public class StatementController : Controller
    {
        private readonly MCBAContext _context;
        public StatementController(MCBAContext context) => _context = context;


        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
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
            var account = await _context.Accounts.FindAsync(HttpContext.Session.GetInt32("AccountHistory"));
            ViewBag.Account = account;
            // Page the orders, maximum of X per page.
            const int pageSize = 4;
            var pagedList = await _context.Transactions.Where(x => x.AccountNumber == account.AccountNumber).
                OrderByDescending(x => x.TransactionTimeUtc).ToPagedListAsync(page, pageSize);

            return View(pagedList);
        }

    }
}
