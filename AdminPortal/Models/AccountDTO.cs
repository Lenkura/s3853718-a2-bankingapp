using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortal.Models
{
    public class AccountDTO
    {
        public enum AccountType
        {
            [Display(Name = "Checking")]
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

            [Column(TypeName = "money")]
            [DataType(DataType.Currency)]
            public decimal Balance { get; set; }
        }
    }
}
