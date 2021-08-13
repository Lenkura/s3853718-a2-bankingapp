using MvcMCBA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using MvcMCBA.Data;
using Microsoft.AspNetCore.Authorization;

namespace MvcMCBA.Controllers
{
    [Authorize(Roles = "Customer")]
    public class PayeeController : Controller
    {
        private readonly MCBAContext _context;
        public PayeeController(MCBAContext context) => _context = context;

        public async Task<IActionResult> PayeeList(int? page = 1)
        {
            // Page the orders, maximum of X per page.
            const int pageSize = 4;
            var pagedList = await _context.Set<Payee>().ToPagedListAsync(page, pageSize);
            return View(pagedList);
        }

        public IActionResult NewPayee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewPayee([Bind("PayeeID,Name,Address,Suburb,State,PostCode,Phone")] Payee payee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(PayeeList));
            }
            return View(payee);
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
        public async Task<IActionResult> EditPayee(int id, [Bind("PayeeID,Name,Address,Suburb,State,PostCode,Phone")] Payee payee)
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
