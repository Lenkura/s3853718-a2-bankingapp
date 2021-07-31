using MvcMCBA.Authorise;
using MvcMCBA.Data;
using MvcMCBA.Models;
using MvcMCBA.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace MvcMCBA.Controllers
{
    [SecureContent]
    public class TransactionController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public TransactionController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var response = await Client.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);
            return View(customer);
        }
        public IActionResult Deposit(int accountNumber)
        {
            var account = GetAccount(accountNumber).Result;
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = account,
                    TransactionType = TransactionType.D
                });
        }

        [HttpPost]
        public IActionResult Deposit(TransactionViewModel viewModel)
        {
            viewModel.Account = GetAccount(viewModel.AccountNumber).Result;
            viewModel = SingleAccountValidation(viewModel);
            if (!ModelState.IsValid)
                return View(viewModel);
            return RedirectToAction(nameof(Confirm), viewModel);
        }
        public IActionResult Withdraw(int accountNumber)
        {
            var account = GetAccount(accountNumber).Result;
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = account,
                    TransactionType = TransactionType.W
                });
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(TransactionViewModel viewModel)
        {
            viewModel.Account = GetAccount(viewModel.AccountNumber).Result;
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
        public IActionResult Transfer(int accountNumber)
        {
            var account = GetAccount(accountNumber).Result;
            return View(
                new TransactionViewModel
                {
                    AccountNumber = accountNumber,
                    Account = account,
                    TransactionType = TransactionType.T
                });
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransactionViewModel viewModel)
        {
            viewModel.Account = GetAccount(viewModel.AccountNumber).Result;
            viewModel.DestinationAccount = GetAccount(viewModel.DestinationAccountNumber).Result;
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
            viewModel.Account = GetAccount(viewModel.AccountNumber).Result;
            viewModel.DestinationAccount = GetAccount(viewModel.DestinationAccountNumber).Result;
            switch (viewModel.TransactionType)
            {
                case (TransactionType.D):
                    viewModel.Account.Balance += viewModel.Amount;
                    var content = new StringContent(JsonConvert.SerializeObject(viewModel.Account), Encoding.UTF8, "application/json");
                    var response = Client.PutAsync("api/Account", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception();

                    var newDeposit = new Transaction
                    {
                        AccountNumber = viewModel.AccountNumber,
                        TransactionType = TransactionType.D,
                        Amount = viewModel.Amount,
                        Comment = viewModel.Comment,
                        TransactionTimeUtc = DateTime.UtcNow

                    };
                     content = new StringContent(JsonConvert.SerializeObject(newDeposit), Encoding.UTF8, "application/json");
                     response = Client.PostAsync("api/Transaction", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception(response.ToString());
                    return RedirectToAction(nameof(Index));

                case (TransactionType.W):
                    var fees = AccountChecks.GetATMFee();
                    if (await HasFreeTransactions(viewModel.Account))
                    {
                        fees = 0;
                    }
                    viewModel.Account.Balance = viewModel.Account.Balance - viewModel.Amount - fees;
                    content = new StringContent(JsonConvert.SerializeObject(viewModel.Account), Encoding.UTF8, "application/json");
                    response = Client.PutAsync("api/Account", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception();

                    var newWithdrawl = new Transaction
                    {
                        AccountNumber = viewModel.AccountNumber,
                        TransactionType = TransactionType.W,
                        Amount = viewModel.Amount,
                        Comment = viewModel.Comment,
                        TransactionTimeUtc = DateTime.UtcNow
                    };
                     content = new StringContent(JsonConvert.SerializeObject(newWithdrawl), Encoding.UTF8, "application/json");
                     response = Client.PostAsync("api/Transaction", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception();

                    if (fees > 0)
                    {
                        var newServiceFee = new Transaction
                        {
                            AccountNumber = viewModel.AccountNumber,
                            TransactionType = TransactionType.S,
                            Amount = fees,
                            Comment = "Withdrawal Service Fee",
                            TransactionTimeUtc = DateTime.UtcNow
                        };
                        content = new StringContent(JsonConvert.SerializeObject(newServiceFee), Encoding.UTF8, "application/json");
                        response = Client.PostAsync("api/Transaction", content).Result;
                        if (!response.IsSuccessStatusCode)
                            throw new Exception();
                    }
                    return RedirectToAction(nameof(Index));

                case (TransactionType.T):
                    fees = AccountChecks.GetTransferFee();
                    if (await HasFreeTransactions(viewModel.Account))
                    {
                        fees = 0;
                    }
                    viewModel.Account.Balance = viewModel.Account.Balance - viewModel.Amount - fees;
                    content = new StringContent(JsonConvert.SerializeObject(viewModel.Account), Encoding.UTF8, "application/json");
                    response = Client.PutAsync("api/Account", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception();

                    viewModel.DestinationAccount.Balance += viewModel.Amount;
                    content = new StringContent(JsonConvert.SerializeObject(viewModel.DestinationAccount), Encoding.UTF8, "application/json");
                    response = Client.PutAsync("api/Account", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception();

                    var transferOut = new Transaction
                    {
                        AccountNumber = viewModel.AccountNumber,
                        TransactionType = TransactionType.T,
                        DestinationAccountNumber = viewModel.DestinationAccountNumber,
                        Amount = viewModel.Amount,
                        Comment = viewModel.Comment,
                        TransactionTimeUtc = DateTime.UtcNow
                    };
                    content = new StringContent(JsonConvert.SerializeObject(transferOut), Encoding.UTF8, "application/json");
                    response = Client.PostAsync("api/Transaction", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception();

                    var transferIn =
                    new Transaction
                    {
                        AccountNumber = viewModel.DestinationAccountNumber,
                        TransactionType = TransactionType.T,
                        Amount = viewModel.Amount,
                        Comment = viewModel.Comment,
                        TransactionTimeUtc = DateTime.UtcNow
                    };
                    content = new StringContent(JsonConvert.SerializeObject(transferIn), Encoding.UTF8, "application/json");
                    response = Client.PostAsync("api/Transaction", content).Result;
                    if (!response.IsSuccessStatusCode)
                        throw new Exception();

                    if (fees > 0) {
                        var serviceFee = new Transaction
                        {
                            AccountNumber = viewModel.AccountNumber,
                            TransactionType = TransactionType.S,
                            Amount = fees,
                            Comment = "Transfer Service Fee",
                            TransactionTimeUtc = DateTime.UtcNow
                        };
                        content = new StringContent(JsonConvert.SerializeObject(serviceFee), Encoding.UTF8, "application/json");
                        response = Client.PostAsync("api/Transaction", content).Result;
                        if (!response.IsSuccessStatusCode)
                            throw new Exception();
                    }
                    return RedirectToAction(nameof(Index));
            }
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
        private async Task<Account> GetAccount(int accountNumber)
        {
            var response = await Client.GetAsync($"api/Account/{accountNumber}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var account = JsonConvert.DeserializeObject<Account>(result);
            return account;
        }

        private async Task<bool> HasFreeTransactions(Account a)
        {
            //var withdrawals = await _context.Transactions.Where(x => x.AccountNumber == a.AccountNumber).Where(x => x.TransactionType == TransactionType.W).CountAsync();
            //var transfers = await _context.Transactions.Where(x => x.AccountNumber == a.AccountNumber).Where(x => x.TransactionType == TransactionType.T).Where(x => x.DestinationAccountNumber != null).CountAsync();
            
            var response = await Client.GetAsync($"api/Transaction/{a.AccountNumber}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(result);

            var withdrawals = new List<Transaction>();
            var transfers = new List<Transaction>();
            foreach (var t in transactionList)
            {
                if (t.TransactionType == TransactionType.W)
                    withdrawals.Add(t);
                if (t.TransactionType == TransactionType.T && t.DestinationAccountNumber != null)
                    transfers.Add(t);
            }

                if (withdrawals.Count + transfers.Count >= AccountChecks.GetFreeTransacionLimit())
                return false;
            else
                return true;
        }
    }
}
