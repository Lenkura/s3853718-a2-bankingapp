using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayeeController : ControllerBase
    {
        private readonly PayeeManager _repo;

        public PayeeController(PayeeManager repo)
        {
            _repo = repo;
        }

        //Get - Retrieve all Payees
        [HttpGet]
        public IEnumerable<Payee> Get()
        {
            return _repo.GetAll();
        }

        // GET{value} - Retrieve a specific payee based on payee ID
        [HttpGet("{id}")]
        public Payee Get(int id)
        {
            return _repo.Get(id);
        }

        // POST - Add a new payee
        [HttpPost]
        public void Post([FromBody] Payee payee)
        {
            _repo.Add(payee);
        }

        // PUT - update an existing payee
        [HttpPut]
        public void Put([FromBody] Payee payee)
        {
            _repo.Update(payee.PayeeID, payee);
        }

        // DELETE - remove an existing payee
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
