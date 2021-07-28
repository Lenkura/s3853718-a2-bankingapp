﻿using Assignment_2.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_2.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<MCBAContext>();

            // Look for customers.
            if (context.Customers.Any())
                return; // DB has already been seeded.

            context.Customers.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    Name = "Matthew Bolger",
                    Address = "123 Fake Street",
                    Suburb = "Melbourne",
                    PostCode = "3000"
                },
                new Customer
                {
                    CustomerID = 2200,
                    Name = "Rodney Cocker",
                    Address = "456 Real Road",
                    Suburb = "Melbourne",
                    PostCode = "3005"
                },
                new Customer
                {
                    CustomerID = 2300,
                    Name = "Shekhar Kalra"
                });

            context.Logins.AddRange(
                new Login
                {
                    LoginID = "12345678",
                    CustomerID = 2100,
                    PasswordHash = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2"
                },
                new Login
                {
                    LoginID = "38074569",
                    CustomerID = 2200,
                    PasswordHash = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04"
                },
                new Login
                {
                    LoginID = "17963428",
                    CustomerID = 2300,
                    PasswordHash = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE"
                });

            context.Accounts.AddRange(
                new Account
                {
                    AccountNumber = 4100,
                    AccountType = AccountType.S,
                    CustomerID = 2100,
                    Balance = 100,
                },
                new Account
                {
                    AccountNumber = 4101,
                    AccountType = AccountType.C,
                    CustomerID = 2100,
                    Balance = 500,

                },
                new Account
                {
                    AccountNumber = 4200,
                    AccountType = AccountType.S,
                    CustomerID = 2200,
                    Balance = 500.95m,
                },
                new Account
                {
                    AccountNumber = 4300,
                    AccountType = AccountType.C,
                    CustomerID = 2300,
                    Balance = 1250.50m,
                });

            const string format = "dd/MM/yyyy hh:mm:ss tt";

            context.Transactions.AddRange(
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = "Opening balance",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 08:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 200,
                    Comment = "First deposit",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 300,
                    Comment = "Second deposit",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 08:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4200,
                    Amount = 500,
                    Comment = "Deposited $500",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 09:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4200,
                    Amount = 0.95m,
                    Comment = "Deposited $0.95",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 09:15:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4300,
                    Amount = 1250.50m,
                    Comment = null,
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 10:00:00 PM", format, null)
                });
            context.Payees.AddRange(
                new Payee
                {
                    Name = "Mr. Baker",
                    Address = "1 Oven Lane",
                    Suburb = "Yeast Alley",
                    State = AusStates.VIC,
                    PostCode = "3500",
                    Phone = "(03) 1234 4568",
                },
                new Payee
                {
                    Name = "Mr. Butcher",
                    Address = "1 Cleaver Way",
                    Suburb = "Mutton Mound",
                    State = AusStates.SA,
                    PostCode = "2516",
                    Phone = "(02) 4286 4894",
                },
                new Payee
                {
                    Name = "Mr. Post",
                    Address = "1 Envelope Street",
                    Suburb = "Parcel Place",
                    State = AusStates.NSW,
                    PostCode = "9999",
                    Phone = "(09) 9999 9999",
                }
                );

            context.SaveChanges();
        }
    }
}