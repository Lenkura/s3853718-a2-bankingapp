using Assignment_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.ViewModels
{
    public class StatementViewModel
    {
        public int AccountNumber { get; set; }
        public int PageNumber { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}
