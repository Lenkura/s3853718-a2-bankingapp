using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMCBA.Data;
using MvcMCBA.Models;
using MvcMCBA.Authorise;
using Microsoft.AspNetCore.Http;
using MvcMCBA.ViewModels;
using SimpleHashing;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace MvcMCBA.Controllers
{
    [SecureContent]
    public class CustomersController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        public CustomersController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;


        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var response = await Client.GetAsync("api/Customer");
            var result = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<Customer>>(result);
            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details()
        {
            var id = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var response = await Client.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);
            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CustomerID,Name,TFN,Address,Suburb,State,PostCode,Mobile")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                var response = Client.PostAsync("api/Customer", content).Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await Client.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CustomerID,Name,TFN,Address,Suburb,State,PostCode,Mobile")] Customer customer)
        {
            if (id != customer.CustomerID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                var response = Client.PutAsync("api/Customer", content).Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Details));
            }
            return View(customer);
        }

        public IActionResult Password()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Password(ChangePasswordViewModel viewModel)
        {
            var loginID = HttpContext.Session.GetString(nameof(Login.LoginID));
            //var login = await _context.Logins.FindAsync(loginID);
            var response = await Client.GetAsync($"api/Login/{loginID}");
            var result = await response.Content.ReadAsStringAsync();
            var login = JsonConvert.DeserializeObject<Login>(result);
            if (login == null || !PBKDF2.Verify(login.PasswordHash, viewModel.OldPasswordHash))
            {
                ModelState.AddModelError(nameof(viewModel.OldPasswordHash), "Incorrect Password");
                return View();
            }

            if (viewModel.NewPasswordHash1 != viewModel.NewPasswordHash2)
            {
                ModelState.AddModelError("PasswordChange", "New Passwords did not Match");
                return View();
            }
            if (!ModelState.IsValid)
                return View();
            login.PasswordHash = PBKDF2.Hash(viewModel.NewPasswordHash1);
            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
            var update = Client.PutAsync("api/Login", content).Result;
            if (update.IsSuccessStatusCode)
                return RedirectToAction("Logout", "Login");
            else
            {
                ModelState.AddModelError("PasswordChange", "Error Updating, try again later");
                return View();
            }

        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await Client.GetAsync($"api/Customer/{id}");
            // if (!response.IsSuccessStatusCode)
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var response = Client.DeleteAsync($"api/Customer/{id}").Result;
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Transaction");
            return NotFound();
        }
    }
}
