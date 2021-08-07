using WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.DataManger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountManager _repo;

        public AccountController(AccountManager repo)
        {
            _repo = repo;
        }

        // Get - Retrieves all Accounts
        [HttpGet]
        public IEnumerable<Account> Get()
        {
            return _repo.GetAll();
        }

        // GET{value} - Retrieves a specific account with the given account number
        [HttpGet("{id}")]
        public Account Get(int id)
        {
            return _repo.Get(id);
        }

        // POST - Adds an account
        [HttpPost]
        public void Post([FromBody] Account account)
        {
            _repo.Add(account);
        }

        // PUT - Updates an account with new details
        [HttpPut]
        public void Put([FromBody] Account account)
        {
            _repo.Update(account.AccountNumber, account);
        }

        // DELETE - Deletes an account with the given account number
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
