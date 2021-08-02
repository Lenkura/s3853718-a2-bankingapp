using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortal.Models.Enums
{
    public enum CustomerStatus
    {
        [Display(Name = "Available")]
        Available = 0,
        [Display(Name = "Blocked")]
        Blocked = 1,
    }

}
