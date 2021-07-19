using Assignment_2.Authorise;
using Assignment_2.Data;
using Assignment_2.Models;
using Assignment_2.ViewModels;
using DataValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.Controllers
{
    public class PayeeController : Controller
    {
        private readonly Assignment2DbContext _context;
        public PayeeController(Assignment2DbContext context) => _context = context;
        public IActionResult PayeeList()
        {
            var payees = _context.Set<Payee>().ToList();
            return View(payees);
        }

        public IActionResult NewPayee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewPayee([Bind("PayeeID,Name,Address,Suburb,State,PostCode,Phone")] PayeeViewModel payeeModel)
        {
            if (ModelState.IsValid)
            {
                var payee = new Payee
                {
                    Name = payeeModel.Name,
                    Address = payeeModel.Address,
                    Suburb = payeeModel.Suburb,
                    State = Enum.Parse<AusStates>(payeeModel.State),
                    PostCode = payeeModel.PostCode,
                    Phone = payeeModel.Phone,
                };
                _context.Add(payee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(PayeeList));
            }
            return View(payeeModel);
        }
        public async Task<IActionResult> EditPayee(int? payeeid)
        {
            if (payeeid == null)
            {
                return NotFound();
            }

            var payee = await _context.Payees.FindAsync(payeeid);
            if (payee == null)
            {
                return NotFound();
            }
            return View(payee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPayee(int id, [Bind("CustomerID,Name,TFN,Address,Suburb,State,PostCode,Mobile")] Payee payee)
        {
            if (id != payee.PayeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayeeExists(payee.PayeeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PayeeList));
            }
            return View(payee);
        }
        private bool PayeeExists(int id)
        {
            return _context.Payees.Any(e => e.PayeeID == id);
        }
    }
}
