using Assignment_2.Data;
using Assignment_2.Models;
using Assignment_2.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Assignment_2.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly Assignment2DbContext _context;
        public TransactionController(ILogger<TransactionController> logger, Assignment2DbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {

            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
            return View(customer);
        }
        public async Task<IActionResult> Deposit(int accountNumber)
        {
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = await _context.Accounts.FindAsync(accountNumber),
                    TransactionType = TransactionType.D.ToString().TransactionTypeExtender()
                });
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);

            viewModel = SingleAccountValidation(viewModel);

            if (!ModelState.IsValid)
                return View(viewModel);
            return RedirectToAction(nameof(Confirm), viewModel);
        }
        public async Task<IActionResult> Withdraw(int accountNumber)
        {
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = await _context.Accounts.FindAsync(accountNumber),
                    TransactionType = TransactionType.W.ToString().TransactionTypeExtender()
                });
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
            viewModel = SingleAccountValidation(viewModel);

            viewModel.Fees = AccountChecks.GetATMFee();
            if (viewModel.Account.FreeTransactions > 0)
            {
                viewModel.Fees = 0;
                viewModel.Account.FreeTransactions -= 1;
            }

            if (viewModel.Account.Balance - viewModel.Amount - viewModel.Fees < AccountChecks.GetAccountTypeMin(viewModel.Account.AccountType.ToString()))
            {
                ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient Funds for Withdrawal");
            }

            if (!ModelState.IsValid)
                return View(viewModel);
            return RedirectToAction(nameof(Confirm), viewModel);
        }
        public async Task<IActionResult> Transfer(int accountNumber)
        {
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = await _context.Accounts.FindAsync(accountNumber),
                    TransactionType = TransactionType.T.ToString().TransactionTypeExtender()
                });
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
            viewModel.DestinationAccount = await _context.Accounts.FindAsync(viewModel.DestinationAccountNumber);
            viewModel = TwoAccountValidation(viewModel);
            viewModel.Fees = AccountChecks.GetTransferFee();
            if (viewModel.Account.FreeTransactions > 0)
            {
                viewModel.Fees = 0;
                viewModel.Account.FreeTransactions -= 1;
            }

            if (viewModel.Account.Balance - viewModel.Amount - viewModel.Fees < AccountChecks.GetAccountTypeMin(viewModel.Account.AccountType.ToString()))
            {
                ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient Funds for Transfer");
            }
            if (!ModelState.IsValid)
                return View(viewModel);
            return RedirectToAction(nameof(Confirm), viewModel);
        }

        public IActionResult Confirm(TransactionViewModel viewModel)
        {
            return View(viewModel);
        }
        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
            viewModel.DestinationAccount = await _context.Accounts.FindAsync(viewModel.DestinationAccountNumber);
            switch (viewModel.TransactionType)
            {
                case ("Deposit"):
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
                case ("Withdrawal"):
                    viewModel.Account.Balance = viewModel.Account.Balance - viewModel.Amount - viewModel.Fees;
                    viewModel.Account.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.W,
                        Amount = viewModel.Amount,
                        Comment = viewModel.Comment,
                        TransactionTimeUtc = DateTime.UtcNow
                    });
                    if (viewModel.Fees > 0)
                        viewModel.Account.Transactions.Add(new Transaction
                        {
                            TransactionType = TransactionType.S,
                            Amount = viewModel.Fees,
                            Comment = "Withdrawal Service Fee",
                            TransactionTimeUtc = DateTime.UtcNow
                        });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                case ("Transfer"):
                    viewModel.Account.Balance = viewModel.Account.Balance - viewModel.Amount - viewModel.Fees;
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

                    if (viewModel.Fees > 0)
                        viewModel.Account.Transactions.Add(new Transaction
                        {
                            TransactionType = TransactionType.S,
                            Amount = viewModel.Fees,
                            Comment = "Transfer Service Fee",
                            TransactionTimeUtc = DateTime.UtcNow
                        });


                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
            }
            Console.WriteLine(viewModel.TransactionType);
            Console.WriteLine(viewModel.AccountNumber);
            Console.WriteLine(viewModel.Amount);
            Console.WriteLine(viewModel.Comment);
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
