using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_2.Models
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

        [Required,StringLength(3)]
        [Column(TypeName = "nvarchar")]
        public AusStates State { get; set; }

        [Required,StringLength(4)]
        [RegularExpression(@"/\d{4}/", ErrorMessage = "Postcodes are 4 digit numbers")]
        public string PostCode { get; set; }

        [Required,StringLength(14)]
        [RegularExpression(@"/^\(?(?:\+?61|0)[2-478]\)?(?:[ -]?[0-9]){8}$/", ErrorMessage = "Please enter an Australian Phone Number")]
        public string Phone { get; set; }
    }
}
