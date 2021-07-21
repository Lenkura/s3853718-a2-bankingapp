using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assignment_2.Models
{
    public enum AccountType
{
        [Display(Name ="Checking")]
        C = 1,
        [Display(Name = "Savings")]
        S = 2
    }
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        [Range(1000, 9999)]
        public int AccountNumber { get; set; }

        [Display(Name = "Type")]
        [Column(TypeName = "char")]
        public AccountType AccountType { get; set; }

        [Display(Name = "Customer ID")]
        [Range(1000, 9999)]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }
}
