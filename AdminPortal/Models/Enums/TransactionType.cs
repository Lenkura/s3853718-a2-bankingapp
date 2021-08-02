using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortal.Models.Enums
{
    public enum TransactionType
    {
        [Display(Name = "Deposit")]
        D = 1,
        [Display(Name = "Withdraw")]
        W = 2,
        [Display(Name = "Transfer")]
        T = 3,
        [Display(Name = "Service")]
        S = 4,
        [Display(Name = "BillPay")]
        B = 5
    }

}
