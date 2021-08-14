using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMCBA.ViewModels
{
    public class TypeBreakdownViewModel
    {
        public decimal Deposit { get; set; }
        public decimal Withdrawal { get; set; }
        public decimal BillPay { get; set; }
        public decimal TransferIn { get; set; }
        public decimal TransferOut { get; set; }
        public decimal Service { get; set; }
    }
}
