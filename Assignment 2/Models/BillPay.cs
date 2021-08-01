using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcMCBA.Models
{
    public enum PaymentPeriod
    {
        [Display(Name = "Monthly")]
        M = 1,
        [Display(Name = "Quarterly")]
        Q = 2,
        [Display(Name = "Annually")]
        Y =3,
        [Display(Name = "One-Off")]
        O =4
    }
    public enum BillPayStatus
    {
        Ready = 1,
        Error = 2,
    }
    public class BillPay
    {
        [Required]
        public int BillPayID { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Account Number")]
        [Range(1000, 9999)]
        public int AccountNumber { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Payee ID")]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }
        [Required]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        [Display(Name = "Scheduled Time (Utc)")]
        public DateTime ScheduleTimeUtc { get; set; }
        [Required]
        [Column(TypeName = "char")]
        public PaymentPeriod Period { get; set; }

        [Column(TypeName = "nvarchar")]
        public BillPayStatus Status { get; set; }
    }
}
