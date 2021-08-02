using AdminPortal.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortal.Models
{

    public class BillPayDTO
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
