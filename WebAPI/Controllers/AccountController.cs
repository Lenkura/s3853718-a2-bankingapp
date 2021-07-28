using Assignment_2.Models;
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

        [HttpGet]
        public IEnumerable<Account> Get()
        {
            return _repo.GetAll();
        }

        // GET{value}
        [HttpGet("{id}")]
        public Account Get(int id)
        {
            return _repo.Get(id);
        }

        // POST 
        [HttpPost]
        public void Post([FromBody] Account account)
        {
            _repo.Add(account);
        }

        // PUT 
        [HttpPut]
        public void Put([FromBody] Account account)
        {
            _repo.Update(account.AccountNumber, account);
        }

        // DELETE 
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
