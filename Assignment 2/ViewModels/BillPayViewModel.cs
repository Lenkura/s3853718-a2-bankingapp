using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Assignment_2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_2.ViewModels
{
    public class BillPayViewModel
    {
        [Required]
        public int BillPayID { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        [Range(1000, 9999)]
        [DataType(DataType.Text)]
        public string AccountNumber { get; set; }
        [Required]
        [Display(Name = "Payee ID")]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }
        [Required]
        [Column(TypeName = "money")]
        [Range(0.01, 99999999999999.99)]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ScheduleTimeUtc { get; set; }
        [Required]
        [Column(TypeName = "char")]
        public string Period { get; set; }
        public List<SelectListItem> AccountNumberList { get; set; }
    }
}
