using Assignment_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.ViewModels
{
    public class DepositViewModel
    {
        public int AccountNumber { get; set; }
        public Account Account { get; set; }
        public decimal Amount { get; set; }
    }
}
}
