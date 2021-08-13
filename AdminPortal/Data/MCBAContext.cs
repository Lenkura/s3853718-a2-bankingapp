using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AdminPortal.Models;

namespace AdminPortal.Data
{
    public class MCBAContext : IdentityDbContext<ApplicationUser>
    {
        public MCBAContext(DbContextOptions<MCBAContext> options)
            : base(options)
        { }

        public DbSet<CustomerDTO> Customers { get; set; }
        public DbSet<AccountDTO> Accounts { get; set; }
        public DbSet<TransactionDTO> Transactions { get; set; }
        public DbSet<BillPayDTO> BillPays { get; set; }
        public DbSet<PayeeDTO> Payees { get; set; }
    }
}
