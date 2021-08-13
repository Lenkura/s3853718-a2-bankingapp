using MvcMCBA.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MvcMCBA.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<MCBAContext>();
            if (context.Customers.Any())
                return; // DB has already been seeded.

            context.Customers.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    Name = "Matthew Bolger",
                    Address = "123 Fake Street",
                    Suburb = "Melbourne",
                    PostCode = "3000",
                    Status = CustomerStatus.A
                },
                new Customer
                {
                    CustomerID = 2200,
                    Name = "Rodney Cocker",
                    Address = "456 Real Road",
                    Suburb = "Melbourne",
                    PostCode = "3005",
                    Status = CustomerStatus.A
                },
                new Customer
                {
                    CustomerID = 2300,
                    Name = "Shekhar Kalra",
                    Status = CustomerStatus.A
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
            var password = new PasswordHasher<ApplicationUser>();
            var user1 = new ApplicationUser
            {
                Email = "12345678",
                NormalizedEmail = "12345678",
                UserName = "12345678",
                NormalizedUserName = "12345678",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                CustomerID = 2100
            };
            var passHash = password.HashPassword(user1, "abc123");
            user1.PasswordHash = passHash;
            var user2 = new ApplicationUser
            {
                Email = "38074569",
                NormalizedEmail = "38074569",
                UserName = "38074569",
                NormalizedUserName = "38074569",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                CustomerID = 2200
            };
            passHash = password.HashPassword(user2, "ilovermit2020");
            user2.PasswordHash = passHash;

            var user3 = new ApplicationUser
            {
                Email = "17963428",
                NormalizedEmail = "17963428",
                UserName = "17963428",
                NormalizedUserName = "17963428",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                CustomerID = 2300
            };
            passHash = password.HashPassword(user3, "youWill_n0tGuess-This!");
            user3.PasswordHash = passHash;
            context.Users.AddRange(user1, user2, user3);

            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (!roleManager.RoleExistsAsync("Customer").Result)
            {
                _ = roleManager.CreateAsync(new IdentityRole("Customer")).Result;
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            _ = userManager.AddToRoleAsync(user1, "Customer").Result;
            _ = userManager.AddToRoleAsync(user2, "Customer").Result;
            _ = userManager.AddToRoleAsync(user3, "Customer").Result;

            var admin = new ApplicationUser
            {
                Email = "admin",
                NormalizedEmail = "admin",
                UserName = "admin",
                NormalizedUserName = "admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };
            passHash = password.HashPassword(admin, "admin");
            admin.PasswordHash = passHash;
            context.Users.Add(admin);
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                _ = roleManager.CreateAsync(new IdentityRole("Admin")).Result;
            }
            _ = userManager.AddToRoleAsync(admin, "Admin").Result;

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
                    Balance = 3079.60M,

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
                },
                 new Transaction
                 {
                     TransactionType = TransactionType.D,
                     AccountNumber = 4101,
                     Amount = 500,
                     Comment = "Wage",
                     TransactionTimeUtc = DateTime.ParseExact("21/05/2021 04:45:00 PM", format, null)
                 },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 400,
                    Comment = "Wage",
                    TransactionTimeUtc = DateTime.ParseExact("22/05/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 200,
                    Comment = "Grocery",
                    TransactionTimeUtc = DateTime.ParseExact("22/05/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 50,
                    Comment = "",
                    TransactionTimeUtc = DateTime.ParseExact("23/05/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 100,
                    Comment = "Food",
                    TransactionTimeUtc = DateTime.ParseExact("24/05/2021 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 20,
                    Comment = "Change",
                    TransactionTimeUtc = DateTime.ParseExact("24/05/2021 08:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = "Wage",
                    TransactionTimeUtc = DateTime.ParseExact("24/05/2021 04:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 100,
                    Comment = "Repairs",
                    TransactionTimeUtc = DateTime.ParseExact("26/05/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 200,
                    Comment = "Food",
                    TransactionTimeUtc = DateTime.ParseExact("26/05/2021 03:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.S,
                    AccountNumber = 4101,
                    Amount = 0.1M,
                    Comment = "ATM Withdrawal",
                    TransactionTimeUtc = DateTime.ParseExact("26/05/2021 03:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = "",
                    TransactionTimeUtc = DateTime.ParseExact("26/05/2021 06:45:00 PM", format, null)
                }, new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 250,
                    Comment = "Payback",
                    TransactionTimeUtc = DateTime.ParseExact("27/05/2021 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.S,
                    AccountNumber = 4101,
                    Amount = 0.1M,
                    Comment = "ATM Withdrawal",
                    TransactionTimeUtc = DateTime.ParseExact("27/05/2021 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 300,
                    Comment = "",
                    TransactionTimeUtc = DateTime.ParseExact("31/05/2021 08:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = "Wage",
                    TransactionTimeUtc = DateTime.ParseExact("05/06/2021 04:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 100,
                    Comment = "",
                    TransactionTimeUtc = DateTime.ParseExact("07/06/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 200,
                    Comment = "Grocery",
                    TransactionTimeUtc = DateTime.ParseExact("09/06/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.S,
                    AccountNumber = 4101,
                    Amount = 0.1M,
                    Comment = "ATM Withdrawal",
                    TransactionTimeUtc = DateTime.ParseExact("09/06/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 50,
                    Comment = "",
                    TransactionTimeUtc = DateTime.ParseExact("11/06/2021 01:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = "Deposited $500",
                    TransactionTimeUtc = DateTime.ParseExact("15/06/2021 09:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 100,
                    Comment = "",
                    TransactionTimeUtc = DateTime.ParseExact("18/06/2021 03:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.W,
                    AccountNumber = 4101,
                    Amount = 200,
                    Comment = "Grocery",
                    TransactionTimeUtc = DateTime.ParseExact("21/06/2021 02:40:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.S,
                    AccountNumber = 4101,
                    Amount = 0.1M,
                    Comment = "ATM Withdrawal",
                    TransactionTimeUtc = DateTime.ParseExact("21/06/2021 02:40:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.D,
                    AccountNumber = 4101,
                    Amount = 350,
                    Comment = "",
                    TransactionTimeUtc = DateTime.ParseExact("23/06/2021 01:45:00 PM", format, null)
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
