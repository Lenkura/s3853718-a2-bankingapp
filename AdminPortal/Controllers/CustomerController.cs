using AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdminPortal.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public CustomerController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> IndexAsync()
        {
            var response = await Client.GetAsync("api/Customer");
            var result = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<CustomerDTO>>(result);
            return View(customers);
        }
    }
}
