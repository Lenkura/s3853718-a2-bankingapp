﻿using MvcMCBA.Models;
using MvcMCBA.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using MvcMCBA.Data;
using Microsoft.AspNetCore.Authorization;

namespace MvcMCBA.Controllers
{
    [Authorize(Roles = "Customer")]
    public class TransactionController : Controller
    {
        private readonly MCBAContext _context;
        public TransactionController(MCBAContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
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
                    TransactionType = TransactionType.D
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
                    TransactionType = TransactionType.W
                });
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
            viewModel = SingleAccountValidation(viewModel);

            var fees = AccountChecks.GetATMFee();
            if (await HasFreeTransactions(viewModel.Account))
            {
                fees = 0;
            }

            if (viewModel.Account.Balance - viewModel.Amount - fees < AccountChecks.GetAccountTypeMin(viewModel.Account.AccountType.ToString()))
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
                    TransactionType = TransactionType.T
                });
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransactionViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
            viewModel.DestinationAccount = await _context.Accounts.FindAsync(viewModel.DestinationAccountNumber);
            viewModel = TwoAccountValidation(viewModel);

            var fees = AccountChecks.GetTransferFee();
            if (await HasFreeTransactions(viewModel.Account))
            {
                fees = 0;
            }

            if (viewModel.Account.Balance - viewModel.Amount - fees < AccountChecks.GetAccountTypeMin(viewModel.Account.AccountType.ToString()))
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
                case (TransactionType.D):
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

                case (TransactionType.W):
                    var fees = AccountChecks.GetATMFee();
                    if (await HasFreeTransactions(viewModel.Account))
                    {
                        fees = 0;
                    }
                    viewModel.Account.Balance = viewModel.Account.Balance - viewModel.Amount - fees;
                    viewModel.Account.Transactions.Add(
                    new Transaction
                    {
                        AccountNumber = viewModel.AccountNumber,
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

                case (TransactionType.T):
                    fees = AccountChecks.GetTransferFee();
                    if (await HasFreeTransactions(viewModel.Account))
                    {
                        fees = 0;
                    }
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
            return RedirectToAction(nameof(Index));
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

        private async Task<bool> HasFreeTransactions(Account a)
        {
            var withdrawals = await _context.Transactions.Where(x => x.AccountNumber == a.AccountNumber).Where(x => x.TransactionType == TransactionType.W).CountAsync();
            var transfers = await _context.Transactions.Where(x => x.AccountNumber == a.AccountNumber).Where(x => x.TransactionType == TransactionType.T).Where(x => x.DestinationAccountNumber != null).CountAsync();
            Console.WriteLine(withdrawals + transfers);
            if (withdrawals + transfers >= AccountChecks.GetFreeTransacionLimit())
                return false;
            else
                return true;
        }
    }
}
