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
    public class Transaction
    {
        [Required]
        public int TransactionID { get; set; }
        [Required]
        [Column(TypeName = "char")]
        public TransactionType TransactionType { get; set; }
        [Required]
        [ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }
        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }
        public virtual Account DestinationAccount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [StringLength(30)]
        public string Comment { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime TransactionTimeUtc { get; set; }
    }
}
