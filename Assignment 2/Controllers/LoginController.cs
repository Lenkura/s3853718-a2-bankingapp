
using MvcMCBA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace MvcMCBA.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public LoginController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;
        public IActionResult Index()
        {
            return View();
        }

        [Route("RequestAccess")]
        public IActionResult Login() => View();


        [HttpPost]
        [Route("RequestAccess")]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            var response = await Client.GetAsync($"api/Login/{loginID}");
            var result = await response.Content.ReadAsStringAsync();
            var login = JsonConvert.DeserializeObject<Login>(result);


            if (login == null || !PBKDF2.Verify(login.PasswordHash, password))
            {
                ModelState.AddModelError("LoginFailed", "Incorrect Username or Password");
                return View(new Login { LoginID = loginID });
            }
            // Login customer.
            var a = await Client.GetAsync($"api/Customer/{login.CustomerID}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var b = await a.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(b);

            HttpContext.Session.SetString(nameof(Models.Login.LoginID), login.LoginID);
            HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
            HttpContext.Session.SetString(nameof(Customer.Name), customer.Name);

            return RedirectToAction("Index", "Transaction");
        }

        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}