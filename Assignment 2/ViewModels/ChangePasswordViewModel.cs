using MvcMCBA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMCBA.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required, StringLength(64)]
        [Display(Name = "Current Password")]
        public string OldPasswordHash { get; set; }
      
        [Required, StringLength(64)]
        [Display(Name = "New Password")]
        public string NewPasswordHash1 { get; set; }
        [Required, StringLength(64)]
        [Display(Name = "Confirm New Password")]
        public string NewPasswordHash2 { get; set; }

    }
}

