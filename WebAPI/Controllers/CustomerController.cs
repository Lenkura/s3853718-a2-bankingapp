using System.Collections.Generic;
using WebAPI.Models;
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

        // Get - Retrieve all customers
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _repo.GetAll();
        }

        // GET{value} - Retrieve a specific customer based on customer ID
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return _repo.Get(id);
        }

        // POST - Add a new customer
        [HttpPost]
        public void Post([FromBody] Customer customer)
        {
            _repo.Add(customer);
        }

        // PUT - update a customers details
        [HttpPut]
        public void Put([FromBody] Customer customer)
        {
            _repo.Update(customer.CustomerID, customer);
        }

        // DELETE - delete a customer
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
