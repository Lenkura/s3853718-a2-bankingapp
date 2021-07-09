using Assignment_2.Data;
using Assignment_2.Models;
using Assignment_2.ViewModels;
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
                new DepositViewModel
                {
                    AccountNumber = accountNumber,
                    Account = await _context.Accounts.FindAsync(accountNumber)
                });
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositViewModel viewModel)
        {
            viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);

            viewModel.Account.Balance += viewModel.Amount;
            viewModel.Account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    Amount = viewModel.Amount,
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
    }
}
