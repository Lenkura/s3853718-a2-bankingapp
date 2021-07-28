using Assignment_2.Data;
using Assignment_2.Models;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Repository;

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
            return _context.Customers.Find(id);
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

        public int Update(int id, Customer movie)
        {
            _context.Update(movie);
            _context.SaveChanges();

            return id;
        }
    }
}
