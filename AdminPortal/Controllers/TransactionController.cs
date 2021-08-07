using AdminPortal.Authorise;
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
    [SecureContent]
    public class TransactionController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public TransactionController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index(string amountMin, string amountMax, int? page = 0)
        {
            var response = await Client.GetAsync($"api/Transaction");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var transactions = JsonConvert.DeserializeObject<List<TransactionDTO>>(result);
            var filteredList = new List<TransactionDTO>();

            decimal min = 0;
            decimal max = decimal.MaxValue;

            if (page == 0 && HttpContext.Session.GetInt32("FilterPageNumber").HasValue)
            {
                HttpContext.Session.Remove("AmountMin");
                HttpContext.Session.Remove("AmountMax");
            }

            HttpContext.Session.SetInt32("FilterPageNumber", page.Value);

            if (HttpContext.Session.GetString("AmountMin") != null)
                min = Decimal.Parse(HttpContext.Session.GetString("AmountMin"));
            if (HttpContext.Session.GetString("AmountMax") != null)
                max = Decimal.Parse(HttpContext.Session.GetString("AmountMax"));

            if (decimal.TryParse(amountMin, out _))
            {
                min = decimal.Parse(amountMin);
                HttpContext.Session.SetString("AmountMin", min.ToString());
            }
            if (decimal.TryParse(amountMax, out _))
            {
                max = decimal.Parse(amountMax);
                HttpContext.Session.SetString("AmountMax", max.ToString());
            }

            foreach (var t in transactions)
                if (t.Amount > min && t.Amount < max)
                    filteredList.Add(t);
            if (page == 0)
                page = 1;
            const int pageSize = 4;
            var pagedList = filteredList.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }

        public IActionResult IndexingOneAccount(int accountNumber)
        {
            HttpContext.Session.Remove("FilterStart");
            HttpContext.Session.Remove("FilterEnd");
            HttpContext.Session.Remove("FilterPageNumber");

            HttpContext.Session.SetInt32("AccountTransactions", accountNumber);
            return RedirectToAction(nameof(OneAccount));
        }

        public async Task<IActionResult> OneAccount(DateTime? DateFilterStart, DateTime? DateFilterEnd, int? page = 0)
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

            if (page == 0 && HttpContext.Session.GetInt32("FilterPageNumber").HasValue)
            {
                HttpContext.Session.Remove("FilterStart");
                HttpContext.Session.Remove("FilterEnd");
            }
            HttpContext.Session.SetInt32("FilterPageNumber", page.Value);

            if (HttpContext.Session.GetString("FilterStart") != null)
                start = DateTime.Parse(HttpContext.Session.GetString("FilterStart"));
            if (HttpContext.Session.GetString("FilterEnd") != null)
                end = DateTime.Parse(HttpContext.Session.GetString("FilterEnd"));

            if (DateFilterStart != null)
            {
                start = DateFilterStart.Value;
                HttpContext.Session.SetString("FilterStart", DateFilterStart.Value.ToShortDateString());
            }

            if (DateFilterEnd != null)
            {
                end = DateFilterEnd.Value;
                HttpContext.Session.SetString("FilterEnd", DateFilterEnd.Value.ToShortDateString());
            }

            foreach (var t in transactions)
                if (start.CompareTo(t.TransactionTimeUtc) < 0 && end.CompareTo(t.TransactionTimeUtc) > 0)
                    filteredList.Add(t);
            if (page == 0)
                page = 1;
            const int pageSize = 4;
            var pagedList = filteredList.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }
    }
}
