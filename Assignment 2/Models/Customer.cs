using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Assignment_2.Models
{
    public enum AusStates
    {
        VIC = 0,
        NSW = 1,
        TAS = 2,
        QLD = 3,
        NT = 4,
        ACT = 5,
        SA = 6,
        WA = 7
    }
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(1000, 9999)]
        public int CustomerID { get; init; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(11)]
        public string TFN { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string Suburb { get; set; }

        [Column(TypeName = "nvarchar")]
        public AusStates State { get; set; }

        [StringLength(4)]
        [Range(1000, 9999, ErrorMessage = "Enter a Valid Postcode")]
        public string PostCode { get; set; }

        [StringLength(12)]
        [RegularExpression(@"04[0-9]{2}\s[0-9]{3}\s[0-9]{3}", ErrorMessage = "Please enter an Australian Phone Number")]
        public string Mobile { get; set; }

        public virtual List<Account> Accounts { get; set; }
        public virtual Login Login { get; set; }
    }
}
