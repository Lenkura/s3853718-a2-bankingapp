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
            HttpContext.Session.SetInt32("AccountTransactions", accountNumber);
            return RedirectToAction(nameof(OneAccount));
        }

        public async Task<IActionResult> OneAccount(int? page = 1)
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountTransactions");

            var response = await Client.GetAsync($"api/Account/{accountNumber}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var account = JsonConvert.DeserializeObject<AccountDTO>(result);
            ViewBag.Account = account;

            response = await Client.GetAsync($"api/Transaction/{accountNumber}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            result = await response.Content.ReadAsStringAsync();
            var transactions = JsonConvert.DeserializeObject<List<TransactionDTO>>(result);
            const int pageSize = 4;
            var pagedList = transactions.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }
    }
}
