using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortal.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Route("RequestAccess")]
        public IActionResult Login(string login, string password)
        {
            if (login == "admin" && password == "admin")
            {
                HttpContext.Session.SetString("login", login);
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                ModelState.AddModelError("LoginFailed", "Incorrect Username or Password (admin/admin)");
                return View("Index");
            }


        }

        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
