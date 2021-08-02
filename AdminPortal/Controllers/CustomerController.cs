using AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortal.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public CustomerController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index()
        {
            var response = await Client.GetAsync("api/Customer");
            var result = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<CustomerDTO>>(result);
            return View(customers);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await Client.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<CustomerDTO>(result);

            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CustomerID,Name,TFN,Address,Suburb,State,PostCode,Mobile")] CustomerDTO customer)
        {
            if (id != customer.CustomerID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                var response = Client.PutAsync("api/Customer", content).Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
    }
}
