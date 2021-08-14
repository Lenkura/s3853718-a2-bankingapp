using MvcMCBA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using MvcMCBA.Data;
using Microsoft.AspNetCore.Authorization;
using MvcMCBA.ViewModels;
using System;
using System.Collections.Generic;

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

        public async Task<IActionResult> ChartTypeBreakdown(int accountNumber)
        {
            var transactions = await _context.Transactions.Where(x => x.AccountNumber == accountNumber).ToListAsync();
            decimal deposits = 0;
            decimal transferIn = 0;
            decimal withdrawals = 0;
            decimal transferOut = 0;
            decimal billpay = 0;
            decimal fees = 0;
            foreach (var t in transactions)
            {
                if (t.TransactionType.Equals(TransactionType.D))
                    deposits += t.Amount;
                else if (t.TransactionType.Equals(TransactionType.W))
                    withdrawals += t.Amount;
                else if (t.TransactionType.Equals(TransactionType.B))
                    billpay += t.Amount;
                else if (t.TransactionType.Equals(TransactionType.T) && t.DestinationAccountNumber != null)
                    transferOut += t.Amount;
                else if (t.TransactionType.Equals(TransactionType.S))
                    fees += t.Amount;
                else
                    transferIn += t.Amount;
            }
            return View(new TypeBreakdownViewModel()
            {
                Deposit = deposits,
                Withdrawal = withdrawals,
                BillPay = billpay,
                TransferIn = transferIn,
                TransferOut = transferOut,
                Service = fees
            });
        }

        public async Task<IActionResult> ChartBalanceTrend(int accountNumber)
        {
            var transactions = await _context.Transactions.Where(x => x.AccountNumber == accountNumber).ToListAsync();

            //hardcode starting date
            const string format = "dd/MM/yyyy";
            DateTime startingDate = DateTime.ParseExact("28/05/2021", format, null);
            List<Transaction> priorTransaction = new();
            List<Transaction> day1 = new();
            List<Transaction> day2 = new();
            List<Transaction> day3 = new();
            List<Transaction> day4 = new();
            List<Transaction> day5 = new();
            List<Transaction> day6 = new();
            List<List<Transaction>> days = new();
            List<AccountTrendViewModel> accountTrend = new();
            foreach (var t in transactions)
            {
                if (t.TransactionTimeUtc.Date.CompareTo(startingDate) <= 0)
                    priorTransaction.Add(t);
                if (t.TransactionTimeUtc.Date.Equals(startingDate.Date.AddDays(1)))
                    day1.Add(t);
                if (t.TransactionTimeUtc.Date.Equals(startingDate.Date.AddDays(2)))
                    day2.Add(t);
                if (t.TransactionTimeUtc.Date.Equals(startingDate.Date.AddDays(3)))
                    day3.Add(t);
                if (t.TransactionTimeUtc.Date.Equals(startingDate.Date.AddDays(4)))
                    day4.Add(t);
                if (t.TransactionTimeUtc.Date.Equals(startingDate.Date.AddDays(5)))
                    day5.Add(t);
                if (t.TransactionTimeUtc.Date.Equals(startingDate.Date.AddDays(6)))
                    day6.Add(t);
            }
            foreach (var day in days) { 
                foreach (var list in days)
                {
                    decimal balance = 0;
                    DateTime date = DateTime.Today;
                    foreach (var t in transactions)
                    {
                        date = t.TransactionTimeUtc.Date;
                        if (t.TransactionType.Equals(TransactionType.D))
                            balance += t.Amount;
                        else if (t.TransactionType.Equals(TransactionType.W))
                            balance -= t.Amount;
                        else if (t.TransactionType.Equals(TransactionType.B))
                            balance -= t.Amount;
                        else if (t.TransactionType.Equals(TransactionType.T) && t.DestinationAccountNumber != null)
                            balance -= t.Amount;
                        else if (t.TransactionType.Equals(TransactionType.S))
                            balance -= t.Amount;
                        else
                            balance += t.Amount;
                    }
                    accountTrend.Add(new AccountTrendViewModel
                    {
                        Date = date,
                        Balance = balance
                    });
                }
            }
            return View(accountTrend);
        }

    }

}
}
