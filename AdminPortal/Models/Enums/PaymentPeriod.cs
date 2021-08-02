using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortal.Models.Enums
{
    public enum PaymentPeriod
    {
        [Display(Name = "Monthly")]
        M = 1,
        [Display(Name = "Quarterly")]
        Q = 2,
        [Display(Name = "Annually")]
        Y = 3,
        [Display(Name = "One-Off")]
        O = 4
    }
}
