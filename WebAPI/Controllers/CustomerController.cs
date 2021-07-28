using System.Collections.Generic;
using Assignment_2.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.DataManger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerManager _repo;

        public CustomerController(CustomerManager repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _repo.GetAll();
        }

        // GET{value}
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return _repo.Get(id);
        }

        // POST 
        [HttpPost]
        public void Post([FromBody] Customer customer)
        {
            _repo.Add(customer);
        }

        // PUT 
        [HttpPut]
        public void Put([FromBody] Customer customer)
        {
            _repo.Update(customer.CustomerID, customer);
        }

        // DELETE 
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
