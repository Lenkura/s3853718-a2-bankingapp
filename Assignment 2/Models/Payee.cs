using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcMCBA.Models
{
    public class Payee
    {
        [Required]
        public int PayeeID { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required,StringLength(50)]
        public string Address { get; set; }

        [Required,StringLength(40)]
        public string Suburb { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        //[StringLength(3)]
        public AusStates State { get; set; }

        [Required,StringLength(4)]
        [Range(1000, 9999, ErrorMessage = "Enter a Valid Postcode")]
        public string PostCode { get; set; }
        [Required]
        [StringLength(14)]
        [RegularExpression(@"\(0[0-9]\)\s[0-9]{4}\s[0-9]{4}", ErrorMessage = "Please enter an Australian Phone Number in the format (0X) XXXX XXXX")]
        public string Phone { get; set; }
    }
}
