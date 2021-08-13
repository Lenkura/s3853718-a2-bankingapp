using AdminPortal.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortal.Models
{


    public class CustomerDTO
    {
        [Key]
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
        public AusStates? State { get; set; }

        [StringLength(4)]
        [Range(1000, 9999, ErrorMessage = "Enter a Valid Postcode")]
        public string PostCode { get; set; }

        [StringLength(12)]
        [RegularExpression(@"04[0-9]{2}\s[0-9]{3}\s[0-9]{3}", ErrorMessage = "Please enter an Australian Phone Number in the format 04XX XXX XXX")]
        public string Mobile { get; set; }

        [Column(TypeName = "nvarchar")]
        public CustomerStatus Status { get; set; }
    }
}
