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
using X.PagedList;

namespace Assignment_2.Controllers
{
    public class BillPayController : Controller
    {
        private readonly Assignment2DbContext _context;
        public BillPayController(Assignment2DbContext context) => _context = context;

        /*  public async Task<IActionResult> List()
          {
              var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
              var billpay = new List<BillPay>();
              foreach (var a in customer.Accounts)
              {
                  var accountBillPays = await _context.BillPays.Where(x => x.AccountNumber == a.AccountNumber).ToListAsync();
                  billpay.AddRange(accountBillPays);
              }
              billpay = billpay.OrderBy(x => x.ScheduleTimeUtc).ToList();
              return View(billpay);
          }*/

        public async Task<IActionResult> List(int? page = 1)
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value);
            var accountNumber = new List<int>();
            foreach (var a in customer.Accounts)
            {
                accountNumber.Add(a.AccountNumber);
            }

            // Page the orders, maximum of X per page.
            const int pageSize = 4;

            var pagedList = await _context.BillPays.Where(x => accountNumber.Contains(x.AccountNumber)).OrderBy(x => x.ScheduleTimeUtc).ToPagedListAsync(page, pageSize);
            return View(pagedList);
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
        public async Task<IActionResult> Cancel(int billpayid)
        {
            var billpay = await _context.BillPays.FindAsync(billpayid);
            return View(billpay);
        }

        [HttpPost, ActionName("Cancel")]
        public async Task<IActionResult> CancelConfirmed(int billpayid)
        {
            var billpay = await _context.BillPays.FindAsync(billpayid);
            _context.BillPays.Remove(billpay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Edit(int billpayid)
        {
            var billpay = await _context.BillPays.FindAsync(billpayid);
            return View(billpay);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BillPay billpay)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billpay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillPayExists(billpay.BillPayID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            return View(billpay);
        }
        private bool BillPayExists(int id)
        {
            return _context.BillPays.Any(e => e.BillPayID == id);
        }

    }
}
