using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMCBA.Models
{
    public class Login
    {
        [Required, Range(10000000,99999999, ErrorMessage = "LoginIds are 8 digit numbers")]
        //[ForeignKey("User")]
        public string LoginID { get; set; }
       // public virtual ApplicationUser User { get; set; }
        [Required, Range(1000, 9999)]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        [Required, StringLength(64)]
        public string PasswordHash { get; set; }

    }
}
