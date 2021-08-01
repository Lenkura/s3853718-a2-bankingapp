using MvcMCBA.Authorise;
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
using X.PagedList;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MvcMCBA.Controllers
{
    public class PayeeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public PayeeController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> PayeeList(int? page = 1)
        {
            // Page the orders, maximum of X per page.
            const int pageSize = 4;

            var response = await Client.GetAsync("api/Payee");
            var result = await response.Content.ReadAsStringAsync();
            var payees = JsonConvert.DeserializeObject<List<Payee>>(result);

            //var pagedList = await _context.Set<Payee>().ToPagedListAsync(page, pageSize);
            var pagedList = payees.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }

        public IActionResult NewPayee()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewPayee([Bind("PayeeID,Name,Address,Suburb,State,PostCode,Phone")] Payee payee)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(payee), Encoding.UTF8, "application/json");
                var response = Client.PostAsync("api/Payee", content).Result;
                if (!response.IsSuccessStatusCode)
                    return View(payee);
                return RedirectToAction(nameof(PayeeList));
            }
            return View(payee);
        }
        public async Task<IActionResult> EditPayee(int? payeeid)
        {
            var response = await Client.GetAsync($"api/Payee/{payeeid}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var payee = JsonConvert.DeserializeObject<Payee>(result);
            if (payee == null)
                return NotFound();
            return View(payee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPayee(int id, [Bind("PayeeID,Name,Address,Suburb,State,PostCode,Phone")] Payee payee)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(payee), Encoding.UTF8, "application/json");
                var response = Client.PutAsync("api/Payee", content).Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(PayeeList));
            }
            return View(payee);
        }
    }
}
