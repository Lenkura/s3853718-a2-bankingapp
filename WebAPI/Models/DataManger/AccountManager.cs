using WebAPI.Data;
using WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManger
{
    public class AccountManager : IBankDataRepository<Account, int>
    {
        private readonly MCBAContext _context;
        public AccountManager(MCBAContext context)
        {
            _context = context;
        }
        public int Add(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return account.AccountNumber;
        }

        public int Delete(int id)
        {
            if (_context.Accounts.Find(id) != null)
            {
                _context.Accounts.Remove(_context.Accounts.Find(id));
                _context.SaveChanges();
            }

            return id;
        }

        public Account Get(int id)
        {
            return _context.Accounts.Find(id);
        }

        public IEnumerable<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public int Update(int id, Account account)
        {
            _context.Update(account);
            _context.SaveChanges();
            return id;
        }
    }
}
