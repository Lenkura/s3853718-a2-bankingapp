﻿using WebAPI.Data;
using WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManger
{
    public class LoginManager
    {
        private readonly MCBAContext _context;

        public LoginManager(MCBAContext context)
        {
            _context = context;
        }

        public Login Get(string id)
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

        public int Delete(string id)
        {
            if (_context.Logins.Find(id) != null)
            {
                _context.Logins.Remove(_context.Logins.Find(id));
                _context.SaveChanges();
            }

            return Int32.Parse(id);
        }

        public int Update(string id, Login login)
        {
            _context.Update(login);
            _context.SaveChanges();
            return Int32.Parse(id);
        }
    }
}
