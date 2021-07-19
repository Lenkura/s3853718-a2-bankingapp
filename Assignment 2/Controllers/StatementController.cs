using Assignment_2.Authorise;
using Assignment_2.Data;
using Assignment_2.Models;
using Assignment_2.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.Controllers
{
    public class StatementController : Controller
    {

        private readonly Assignment2DbContext _context;
        public StatementController( Assignment2DbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
            return View(customer);
        }

        public async Task<IActionResult> History(int accountNumber)
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
        }

    }
}
