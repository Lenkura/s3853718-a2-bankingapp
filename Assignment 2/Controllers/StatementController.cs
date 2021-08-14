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
            const string format = "dd/MM/yyyy hh:mm:ss tt";
            DateTime startingDate = DateTime.ParseExact("21/05/2021 11:59:59 PM", format, null);

            List<Transaction> priorTransaction = new();
            List<Transaction> day1 = new();
            List<Transaction> day2 = new();
            List<Transaction> day3 = new();
            List<Transaction> day4 = new();
            List<Transaction> day5 = new();
            List<Transaction> day6 = new();

            foreach (var t in transactions)
            {
                if (t.TransactionTimeUtc.CompareTo(startingDate) <= 0)
                    priorTransaction.Add(t);
                if (t.TransactionTimeUtc.ToLocalTime().CompareTo(startingDate.AddDays(1)) <= 0)
                    day1.Add(t);
                if (t.TransactionTimeUtc.ToLocalTime().CompareTo(startingDate.AddDays(2)) <= 0)
                    day2.Add(t);
                if (t.TransactionTimeUtc.ToLocalTime().CompareTo(startingDate.AddDays(3)) <= 0)
                    day3.Add(t);
                if (t.TransactionTimeUtc.ToLocalTime().CompareTo(startingDate.AddDays(4)) <= 0)
                    day4.Add(t);
                if (t.TransactionTimeUtc.ToLocalTime().CompareTo(startingDate.AddDays(5)) <= 0)
                    day5.Add(t);
                if (t.TransactionTimeUtc.ToLocalTime().CompareTo(startingDate.AddDays(6)) <= 0)
                    day6.Add(t);
            }

            var accountTrend = new List<AccountTrendViewModel>(){
            new AccountTrendViewModel
            {
                Date = startingDate.Date,
                Balance = TransactionListTotal(priorTransaction)
            },
            new AccountTrendViewModel
            {
                Date = startingDate.Date.AddDays(1),
                Balance = TransactionListTotal(day1)
            },
            new AccountTrendViewModel
            {
                Date = startingDate.Date.AddDays(2),
                Balance = TransactionListTotal(day2)
            },
            new AccountTrendViewModel
            {
                Date = startingDate.Date.AddDays(3),
                Balance = TransactionListTotal(day3)
            },
            new AccountTrendViewModel
            {
                Date = startingDate.Date.AddDays(4),
                Balance = TransactionListTotal(day4)
            },
            new AccountTrendViewModel
            {
                Date = startingDate.Date.AddDays(5),
                Balance = TransactionListTotal(day5)
            },
            new AccountTrendViewModel
            {
                Date = startingDate.Date.AddDays(6),
                Balance = TransactionListTotal(day6)
            },

            };
            return View(accountTrend);
        }

        private decimal TransactionListTotal(List<Transaction> transactions)
        {
            decimal balance = 0;
            foreach (var t in transactions)
            {
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
            return balance;
        }

    }
}
