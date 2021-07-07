using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_2.Models
{
    public enum PaymentPeriod
    {
        M = 1,
        Q = 2,
        Y =3,
        O=4
    }
    public class BillPay
    {
        [Required]
        public int BillPayID { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        [Range(1000, 9999)]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }
        [Required]
        [Display(Name = "Customer ID")]
        [Range(1000, 9999)]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }
        [Required]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ScheduleTimeUtc { get; set; }
        [Required]
        [Column(TypeName = "char")]
        public PaymentPeriod Period { get; set; }
    }
}
