using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.Models
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
        [ForeignKey("Account")]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; init; }
        public virtual Account Account { get; init; }
        [ForeignKey("DestinationAccount")]
        [Display(Name = "Destination Account")]
        public int? DestinationAccountNumber { get; init; }
        public virtual Account DestinationAccount { get; init; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Amount { get; init; }

        [StringLength(30)]
        public string Comment { get; init; }
        [Required]
        [Column(TypeName = "datetime2")]
        [Display(Name = "Time")]
        public DateTime TransactionTimeUtc { get; init; }
    }
}
