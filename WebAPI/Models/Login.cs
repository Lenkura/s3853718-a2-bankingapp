using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Login
    {
        [Required, Range(10000000,99999999, ErrorMessage = "LoginIds are 8 digit numbers")]
        public string LoginID { get; set; }
        [Required, Range(1000, 9999)]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        [Required, StringLength(64)]
        public string PasswordHash { get; set; }

    }
}
