using MvcMCBA.Models;
using MvcMCBA.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using MvcMCBA.Authorise;
using MvcMCBA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MvcMCBA.Controllers
{
    [Authorize(Roles = "Customer")]
    public class BillPayController : Controller
    {
        private readonly MCBAContext _context;
        public BillPayController(MCBAContext context) => _context = context;

        public async Task<IActionResult> List(int? page = 1)
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32("CustomerID").Value);
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

        public async Task<IActionResult> NewPayment(int payeeid)
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32("CustomerID").Value);
            var accountNumber = new List<SelectListItem>();
            foreach (var a in customer.Accounts)
            {
                accountNumber.Add(new SelectListItem { Value = a.AccountNumber.ToString(), Text = a.AccountNumber.ToString() });
            }
            BillPayViewModel billpayModel = new()
            {
                PayeeID = payeeid,
                AccountNumberList = accountNumber
            };
            return View(billpayModel);
        }
        [HttpPost]
        public async Task<IActionResult> NewPayment(BillPayViewModel viewModel)
        {
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32("CustomerID").Value);
            bool ownAccount = false;
            foreach (var a in customer.Accounts)
                if (a.AccountNumber == Int32.Parse(viewModel.AccountNumber))
                    ownAccount = true;
            if (!ownAccount)
                ModelState.AddModelError(nameof(viewModel.AccountNumber), "Enter one of your Accounts");
            if (DateTime.Compare(viewModel.ScheduleTimeUtc, DateTime.Now) < 0)
                ModelState.AddModelError(nameof(viewModel.ScheduleTimeUtc), "Set a future time");
            if (!viewModel.Amount.ToString().IsDollarAmount())
                ModelState.AddModelError(nameof(viewModel.Amount), "Enter a dollar amount");



            if (!ModelState.IsValid)
            {
                var accountNumber = new List<SelectListItem>();
                foreach (var a in customer.Accounts)
                {
                    accountNumber.Add(new SelectListItem { Value = a.AccountNumber.ToString(), Text = a.AccountNumber.ToString() });
                }
                BillPayViewModel billpayModel = new()
                {
                    AccountNumber = viewModel.AccountNumber,
                    PayeeID = viewModel.PayeeID,
                    Amount = viewModel.Amount,
                    ScheduleTimeUtc = viewModel.ScheduleTimeUtc,
                    Period = viewModel.Period,
                    AccountNumberList = accountNumber
                };
                return View(billpayModel);
            }
               

            BillPay billpay = new()
            {
                AccountNumber = Int32.Parse(viewModel.AccountNumber),
                PayeeID = viewModel.PayeeID,
                Amount = viewModel.Amount,
                ScheduleTimeUtc = viewModel.ScheduleTimeUtc.ToUniversalTime(),
                Period = Enum.Parse<PaymentPeriod>(viewModel.Period),
                Status = BillPayStatus.R,
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
            var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32("CustomerID").Value);
            bool ownAccount = false;
            foreach (var a in customer.Accounts)
                if (a.AccountNumber == billpay.AccountNumber)
                    ownAccount = true;
            if (!ownAccount)
                ModelState.AddModelError(nameof(billpay.AccountNumber), "Enter one of your Accounts");
            if (DateTime.Compare(billpay.ScheduleTimeUtc, DateTime.Now) < 0)
                ModelState.AddModelError(nameof(billpay.ScheduleTimeUtc), "Set a future time");
            if (!billpay.Amount.ToString().IsDollarAmount())
                ModelState.AddModelError(nameof(billpay.Amount), "Enter a dollar amount");

            if (ModelState.IsValid)
            {
                try
                {
                    billpay.Status = BillPayStatus.R;
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
            var accountNumber = new List<SelectListItem>();
            foreach (var a in customer.Accounts)
            {
                accountNumber.Add(new SelectListItem { Value = a.AccountNumber.ToString(), Text = a.AccountNumber.ToString() });
            }
            BillPayViewModel billpayModel = new()
            {
                AccountNumber = billpay.AccountNumber.ToString(),
                PayeeID = billpay.PayeeID,
                Amount = billpay.Amount,
                ScheduleTimeUtc = billpay.ScheduleTimeUtc,
                Period = billpay.Period.ToString(),
                AccountNumberList = accountNumber
            };
            return View(billpay);
        }
        private bool BillPayExists(int id)
        {
            return _context.BillPays.Any(e => e.BillPayID == id);
        }
    }
}
