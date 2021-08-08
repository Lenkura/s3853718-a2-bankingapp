using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortal.Models.Enums
{
    public enum BillPayStatus
    {
        [Display(Name = "Ready")]
        R = 1,
        [Display(Name = "Error")]
        E = 2,
        [Display(Name = "Blocked")]
        B = 3,
    }
}
