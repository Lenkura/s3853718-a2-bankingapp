using Assignment_2.Authorise;
using Assignment_2.Data;
using Assignment_2.Models;
using Assignment_2.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.Controllers
{
    public class BillPayController : Controller
    {
        private readonly Assignment2DbContext _context;
        public BillPayController(Assignment2DbContext context) => _context = context;

        public async Task<IActionResult> List()
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
            //var billpay = await _context.BillPays.FromSqlRaw(@"select b.* from BillPay b where AccountNumber").ToListAsync();
            var billpay = await _context.BillPays.Where(x => x.AccountNumber == customer.Accounts[0].AccountNumber).ToListAsync();
            return View(billpay);
        }
        public IActionResult NewPayment(int payeeid)
        {
            BillPayViewModel billpayModel = new()
            {
                PayeeID = payeeid,
            };
            return View(billpayModel);
        }
        [HttpPost]
        public async Task<IActionResult> NewPayment(BillPayViewModel viewModel)
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
            bool ownAccount = false;
            foreach (var a in customer.Accounts)
                if (a.AccountNumber == viewModel.AccountNumber)
                    ownAccount = true;
            if (!ownAccount)
                ModelState.AddModelError(nameof(viewModel.AccountNumber), "Account not Found");
            if (DateTime.Compare(viewModel.ScheduleTimeUtc, DateTime.Now) < 0)
            {
                ModelState.AddModelError(nameof(viewModel.ScheduleTimeUtc), "Set a future time");
            }

            if (!ModelState.IsValid)
                return View(viewModel);
            

            BillPay billpay = new()
            {
                AccountNumber = viewModel.AccountNumber,
                PayeeID = viewModel.PayeeID,
                Amount = viewModel.Amount,
                ScheduleTimeUtc = viewModel.ScheduleTimeUtc.ToUniversalTime(),
                Period = Enum.Parse<PaymentPeriod>(viewModel.Period),
            };
            _context.Add(billpay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }



    }
}
