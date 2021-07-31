using WebAPI.Data;
using WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManger
{
    public class TransactionManager : IBankDataRepository<Transaction, int>
    {
        private readonly MCBAContext _context;

        public TransactionManager(MCBAContext context)
        {
            _context = context;
        }

        public Transaction Get(int id)
        {
            return _context.Transactions.Find(id);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _context.Transactions.ToList();
        }

        public IEnumerable<Transaction> GetAccount(int accountNumber)
        {
            return _context.Transactions.Where(x => x.AccountNumber == accountNumber).
                OrderByDescending(x => x.TransactionTimeUtc).ToList();
        }

        public int Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction.TransactionID;
        }

        public int Delete(int id)
        {
            if (_context.Transactions.Find(id) != null)
            {
                _context.Transactions.Remove(_context.Transactions.Find(id));
                _context.SaveChanges();
            }

            return id;
        }

        public int Update(int id, Transaction transaction)
        {
            _context.Update(transaction);
            _context.SaveChanges();
            return id;
        }
    }
}
