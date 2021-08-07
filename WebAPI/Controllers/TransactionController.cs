using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionManager _repo;

        public TransactionController(TransactionManager repo)
        {
            _repo = repo;
        }

        //Get - Retrieve all transactions
        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            return _repo.GetAll();
        }

        // GET{value} - Retrieve all transactions of a specfic account, specified by the account number
        [HttpGet("{accountNumber}")]
        public IEnumerable<Transaction> Get(int accountNumber)
        {
            return _repo.GetAccount(accountNumber);
        }

        // POST - Add a new transaction
        [HttpPost]
        public void Post([FromBody] Transaction transaction)
        {
            _repo.Add(transaction);
        }

        // PUT - update an existing transaction
        [HttpPut]
        public void Put([FromBody] Transaction transaction)
        {
            _repo.Update(transaction.TransactionID, transaction);
        }

        // DELETE - delete an existing transaction 
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
