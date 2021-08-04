using AdminPortal.Models;
using AdminPortal.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
    public class BillPayController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public BillPayController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index(int? page = 1)
        {
            var response = await Client.GetAsync($"api/BillPay");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var billpays = JsonConvert.DeserializeObject<List<BillPayDTO>>(result);

            const int pageSize = 4;
            var pagedList = billpays.ToPagedList((int)page, pageSize);
            return View(pagedList);
        }

        public async Task<IActionResult> BillPayStatusChange(int id, BillPayStatus target)
        {
            var response = await Client.GetAsync($"api/BillPay/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var billpay = JsonConvert.DeserializeObject<BillPayDTO>(result);

            billpay.Status = target;

            var content = new StringContent(JsonConvert.SerializeObject(billpay), Encoding.UTF8, "application/json");
            response = Client.PutAsync("api/Billpay", content).Result;

            return RedirectToAction(nameof(Index));
        }


        /*public async Task<IActionResult> Edit(int billpayid)
        {
            var response = await Client.GetAsync($"api/BillPay/{billpayid}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var billpay = JsonConvert.DeserializeObject<BillPayDTO>(result);
            return View(billpay);
        }

        [HttpPost]
        public IActionResult Edit(BillPayDTO billpay)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(billpay), Encoding.UTF8, "application/json");
                var response = Client.PutAsync("api/BillPay", content).Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }
            return View(billpay);
        }*/
    }
}
