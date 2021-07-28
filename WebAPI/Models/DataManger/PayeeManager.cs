using WebAPI.Data;
using WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManger
{
    public class PayeeManager : IBankDataRepository<Payee, int>
    {
        private readonly MCBAContext _context;

        public PayeeManager(MCBAContext context)
        {
            _context = context;
        }

        public Payee Get(int id)
        {
            return _context.Payees.Find(id);
        }

        public IEnumerable<Payee> GetAll()
        {
            return _context.Payees.ToList();
        }

        public int Add(Payee payee)
        {
            _context.Payees.Add(payee);
            _context.SaveChanges();

            return payee.PayeeID;
        }

        public int Delete(int id)
        {
            if (_context.Payees.Find(id) != null)
            {
                _context.Payees.Remove(_context.Payees.Find(id));
                _context.SaveChanges();
            }

            return id;
        }

        public int Update(int id, Payee payee)
        {
            _context.Update(payee);
            _context.SaveChanges();
            return id;
        }
    }
}
