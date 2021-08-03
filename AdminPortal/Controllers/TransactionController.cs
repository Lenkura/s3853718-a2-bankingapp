using AdminPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace AdminPortal.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public TransactionController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index(int? page = 1)
        {
            var response = await Client.GetAsync($"api/Transaction");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var transactions = JsonConvert.DeserializeObject<List<TransactionDTO>>(result);
            const int pageSize = 4;
            var pagedList = transactions.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }

        public IActionResult IndexingOneAccount(int accountNumber)
        {
            HttpContext.Session.Remove("FilterStart");
            HttpContext.Session.Remove("FilterEnd");

            HttpContext.Session.SetInt32("AccountTransactions", accountNumber);
            return RedirectToAction(nameof(OneAccount),
                new DateFilterViewModel
                {
                    Start = DateTime.MinValue,
                    End = DateTime.Now
                }
                );
        }

        public async Task<IActionResult> OneAccount(int? page = 1)
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountTransactions");
            ViewBag.Account = accountNumber;

            var response = await Client.GetAsync($"api/Transaction/{accountNumber}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var transactions = JsonConvert.DeserializeObject<List<TransactionDTO>>(result);
            var filteredList = new List<TransactionDTO>();

            var start = DateTime.MinValue;
            var end = DateTime.Now;

            if (HttpContext.Session.GetString("FilterStart") != null)
                start = DateTime.Parse(HttpContext.Session.GetString("FilterStart"));
            if (HttpContext.Session.GetString("FilterEnd") != null)
                end = DateTime.Parse(HttpContext.Session.GetString("FilterEnd"));

            foreach (var t in transactions)
                if (start.CompareTo(t.TransactionTimeUtc) < 0 && end.CompareTo(t.TransactionTimeUtc) > 0)
                    filteredList.Add(t);

            const int pageSize = 4;
            var pagedList = filteredList.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }

        public IActionResult DateFilter()
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountTransactions");
            ViewBag.Account = accountNumber;
            return View();
        }

        [HttpPost]
        public IActionResult DateFilter(DateFilterViewModel filter)
        {
            if (filter.Start == null)
                filter.Start = DateTime.MinValue;
            if (filter.End == null)
                filter.End = DateTime.Now;

            if (!(filter.Start.Value.CompareTo(DateTime.Now) < 0))
                ModelState.AddModelError(nameof(filter.End), "Start date must be before current date");

            if (!(filter.Start.Value.CompareTo(filter.End) < 0))
                ModelState.AddModelError(nameof(filter.End), "End date should be after start date");
            if (!ModelState.IsValid)
                return View(filter);
            HttpContext.Session.SetString("FilterStart", filter.Start.ToString());
            HttpContext.Session.SetString("FilterEnd", filter.End.ToString());
            return RedirectToAction(nameof(OneAccount));
        }
    }
}
