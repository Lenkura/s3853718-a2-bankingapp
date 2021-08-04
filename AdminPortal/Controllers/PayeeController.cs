using AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using X.PagedList;

namespace AdminPortal.Controllers
{
    public class PayeeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public PayeeController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index(int? page=1)
        {
            var response = await Client.GetAsync($"api/Payee");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var payee = JsonConvert.DeserializeObject<List<PayeeDTO>>(result);

            const int pageSize = 4;
            var pagedList = payee.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }
    }
}
