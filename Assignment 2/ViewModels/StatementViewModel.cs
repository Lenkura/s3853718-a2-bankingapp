using MvcMCBA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMCBA.ViewModels
{
    public class StatementViewModel
    {
        public int AccountNumber { get; set; }
        public int PageNumber { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}
