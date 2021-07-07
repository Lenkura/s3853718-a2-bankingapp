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
        SA =6,
        WA = 7
    }
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(1000, 9999)]
        public int CustomerID { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [StringLength(11)]
        public string TFN { get; set; }
        [StringLength(50)]
        public string Address { get; set; }
        [StringLength(40)]
        public string Suburb { get; set; }
        [StringLength(3)]
        [EnumDataType(typeof(AusStates), ErrorMessage = "Postcodes are 4 digit numbers")]
        public string State { get; set; }
        [StringLength(4)]
        [RegularExpression(@"/\d{4}/", ErrorMessage = "Postcodes are 4 digit numbers")]
        public string PostCode { get; set; }
        [StringLength(12)]
        [RegularExpression(@"/^(?:\+?61|0)[2-478](?:[ -]?[0-9]){8}$/", ErrorMessage = "Please enter an Australian Phone Number")]
        public string Mobile { get; set; }

        public virtual List<Account> Accounts { get; set; }
        public virtual Login Login { get; set; }
    }
}
