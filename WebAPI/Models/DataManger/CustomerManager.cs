using WebAPI.Data;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models.DataManger
{
    public class CustomerManager : IBankDataRepository<Customer, int>
    {
        private readonly MCBAContext _context;

        public CustomerManager(MCBAContext context)
        {
            _context = context;
        }

        public Customer Get(int id)
        {
            return _context.Customers.Include(c => c.Accounts).ThenInclude(a=>a.Transactions).FirstOrDefault(c=>c.CustomerID==id);
           // return _context.Customers.Find(id);
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }
            
        public int Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return customer.CustomerID;
        }

        public int Delete(int id)
        {
            if (_context.Customers.Find(id) != null)
            {
                _context.Customers.Remove(_context.Customers.Find(id));
                _context.SaveChanges();
            }

            return id;
        }

        public int Update(int id, Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
            return id;
        }
    }
}
