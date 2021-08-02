using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortal.Models
{
    public class TransactionDTO
    {
        public enum TransactionType
        {
            [Display(Name = "Deposit")]
            D = 1,
            [Display(Name = "Withdraw")]
            W = 2,
            [Display(Name = "Transfer")]
            T = 3,
            [Display(Name = "Service")]
            S = 4,
            [Display(Name = "BillPay")]
            B = 5
        }
        public record Transaction
        {
            [Required]
            [Display(Name = "ID")]
            public int TransactionID { get; init; }
            [Required]
            [Column(TypeName = "char")]
            [Display(Name = "Type")]
            public TransactionType TransactionType { get; init; }
            [Required]
            [Display(Name = "Account Number")]
            public int AccountNumber { get; init; }
            [Display(Name = "Destination Account")]
            public int? DestinationAccountNumber { get; init; }
            [Required]
            [Column(TypeName = "money")]
            [DataType(DataType.Currency)]
            public decimal Amount { get; init; }
            [StringLength(30)]
            public string Comment { get; init; }
            [Required]
            [Column(TypeName = "datetime2")]
            [Display(Name = "Time")]
            public DateTime TransactionTimeUtc { get; init; }
        }
    }
}
