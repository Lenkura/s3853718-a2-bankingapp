using Assignment_2.Data;
using Assignment_2.Models;
using Assignment_2.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Assignment_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Assignment2DbContext _context;

        public HomeController(ILogger<HomeController> logger, Assignment2DbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {

            var customer = await _context.Customers.FindAsync(2100);
            return View(customer);
        }
        public async Task<IActionResult> Deposit(int accountNumber)
        {
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = await _context.Accounts.FindAsync(accountNumber)
                });
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);

            viewModel = SingleAccountValidation(viewModel);

            if (!ModelState.IsValid)
                return View(viewModel);
            viewModel.Account.Balance += viewModel.Amount;
            viewModel.Account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    Amount = viewModel.Amount,
                    Comment = viewModel.Comment,
                    TransactionTimeUtc = DateTime.UtcNow

                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Withdraw(int accountNumber)
        {
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = await _context.Accounts.FindAsync(accountNumber)
                });
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
            viewModel = SingleAccountValidation(viewModel);

            decimal fees = AccountChecks.GetATMFee();
            if (viewModel.Account.FreeTransactions > 0)
            {
                fees = 0;
                viewModel.Account.FreeTransactions -= 1;
            }

            if (viewModel.Account.Balance - viewModel.Amount - fees < AccountChecks.GetAccountTypeMin(viewModel.Account.AccountType.ToString()))
            {
                ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient Funds for Withdrawal");
            }

            if (!ModelState.IsValid)
                return View(viewModel);

            viewModel.Account.Balance = viewModel.Account.Balance - viewModel.Amount - fees;
            viewModel.Account.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.W,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
            });
            if (fees > 0)
                viewModel.Account.Transactions.Add(new Transaction
                {
                    TransactionType = TransactionType.S,
                    Amount = fees,
                    Comment = "Withdrawal Service Fee",
                    TransactionTimeUtc = DateTime.UtcNow
                });


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Transfer(int accountNumber)
        {
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = await _context.Accounts.FindAsync(accountNumber)
                });
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
            viewModel.DestinationAccount = await _context.Accounts.FindAsync(viewModel.DestinationAccountNumber);
            viewModel = TwoAccountValidation(viewModel);
            decimal fees = AccountChecks.GetTransferFee();
            if (viewModel.Account.FreeTransactions > 0)
            {
                fees = 0;
                viewModel.Account.FreeTransactions -= 1;
            }

            if (viewModel.Account.Balance - viewModel.Amount - fees < AccountChecks.GetAccountTypeMin(viewModel.Account.AccountType.ToString()))
            {
                ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient Funds for Transfer");
            }
            if (!ModelState.IsValid)
                return View(viewModel);

            viewModel.Account.Balance = viewModel.Account.Balance - viewModel.Amount - fees;
            viewModel.DestinationAccount.Balance += viewModel.Amount;
            viewModel.Account.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.T,
                DestinationAccountNumber = viewModel.DestinationAccountNumber,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
            });
            viewModel.DestinationAccount.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.T,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
             });

            if (fees > 0)
                viewModel.Account.Transactions.Add(new Transaction
                {
                    TransactionType = TransactionType.S,
                    Amount = fees,
                    Comment = "Transfer Service Fee",
                    TransactionTimeUtc = DateTime.UtcNow
                });


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private TransactionViewModel SingleAccountValidation(TransactionViewModel tvm)
        {
            if (!tvm.Amount.ToString().IsDollarAmount())
            {
                ModelState.AddModelError(nameof(tvm.Amount), "Enter a dollar amount");
            }
            return tvm;
        }
        private TransactionViewModel TwoAccountValidation(TransactionViewModel tvm)
        {
            if (tvm.DestinationAccountNumber == tvm.AccountNumber)
            {
                ModelState.AddModelError(nameof(tvm.DestinationAccountNumber), "Can not Transfer to same account");
            }
            
            if (tvm.DestinationAccount == null)
            {
                ModelState.AddModelError(nameof(tvm.DestinationAccountNumber), "Account not Found");
            }

            if (!tvm.Amount.ToString().IsDollarAmount())
            {
                ModelState.AddModelError(nameof(tvm.Amount), "Enter a dollar amount");
            }
            return tvm;
        }
    }
}
