using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcMCBA.Models
{
    public class PayeeViewModel
    {
        [Required]
        public int PayeeID { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required, StringLength(50)]
        public string Address { get; set; }

        [Required, StringLength(40)]
        public string Suburb { get; set; }

        [Required, StringLength(3)]
        [Column(TypeName = "nvarchar")]
        public string State { get; set; }

        [Required, StringLength(4)]
        [Range(1000, 9999, ErrorMessage = "Enter a Valid Postcode")]
        public string PostCode { get; set; }
        [Required]
        [StringLength(12)]
        [RegularExpression(@"04[0-9]{2}\s?[0-9]{3}\s?[0-9]{3}", ErrorMessage = "Please enter an Australian Phone Number")]
        public string Phone { get; set; }
    }
}
