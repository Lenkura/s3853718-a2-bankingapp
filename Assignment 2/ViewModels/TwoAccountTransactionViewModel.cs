﻿using Assignment_2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.ViewModels
{
    public class TwoAccountTransactionViewModel
    {
        public int AccountNumber { get; set; }
        public Account Account { get; set; }
        [Required]
        [Range(1000, 9999, ErrorMessage = "Account not Found")]
        [DataType(DataType.Text)]
        public int DestinationAccountNumber { get; set; }
        public Account DestinationAccount { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Enter a positive amount")]
        public decimal Amount { get; set; }
        [StringLength(30)]
        public string Comment { get; set; }
    }
}
