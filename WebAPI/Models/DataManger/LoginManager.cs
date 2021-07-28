using WebAPI.Data;
using WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManger
{
    public class LoginManager : IBankDataRepository<Login, int>
    {
        private readonly MCBAContext _context;

        public LoginManager(MCBAContext context)
        {
            _context = context;
        }

        public Login Get(int id)
        {
            return _context.Logins.Find(id);
        }

        public IEnumerable<Login> GetAll()
        {
            return _context.Logins.ToList();
        }

        public int Add(Login login)
        {
            _context.Logins.Add(login);
            _context.SaveChanges();

            return Int32.Parse(login.LoginID);
        }

        public int Delete(int id)
        {
            if (_context.Logins.Find(id) != null)
            {
                _context.Logins.Remove(_context.Logins.Find(id));
                _context.SaveChanges();
            }

            return id;
        }

        public int Update(int id, Login login)
        {
            _context.Update(login);
            _context.SaveChanges();
            return id;
        }
    }
}
