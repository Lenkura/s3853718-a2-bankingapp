using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMCBA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
