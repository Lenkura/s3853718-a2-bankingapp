
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
using Microsoft.AspNetCore.Authorization;
using MvcMCBA.Data;
using Microsoft.AspNetCore.Identity;

namespace MvcMCBA.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly MCBAContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public LoginController(MCBAContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }
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
            var login = await _context.Logins.FindAsync(loginID);
            if (login == null || !PBKDF2.Verify(login.PasswordHash, password))
            {
                ModelState.AddModelError("LoginFailed", "Incorrect Username or Password");
                return View(new Login { LoginID = loginID });
            }
            var customer = await _context.Customers.FindAsync(login.CustomerID);
            if (customer.Status == CustomerStatus.B)
            {
                ModelState.AddModelError("LoginFailed", "Account Locked - Please contact us");
                return View(new Login { LoginID = loginID });
            }
            // Login customer.
            HttpContext.Session.SetString("LoginID", login.LoginID);
            HttpContext.Session.SetInt32("CustomerID", login.CustomerID);
            HttpContext.Session.SetString("Name", login.Customer.Name);

            return RedirectToAction("Index", "Transaction");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        public IActionResult Registration()
        {
            return View();
        }
    }
}