using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortal.Models.Enums
{

    public enum AccountType
    {
        [Display(Name = "Checking")]
        C = 1,
        [Display(Name = "Savings")]
        S = 2
    }

}
