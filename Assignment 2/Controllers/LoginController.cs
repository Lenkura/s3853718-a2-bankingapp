
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