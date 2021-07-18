using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.Controllers
{
    public class BillPayController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
