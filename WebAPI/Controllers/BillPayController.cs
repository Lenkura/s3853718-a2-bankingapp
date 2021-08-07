using System.Collections.Generic;
using WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.DataManger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillPayController : ControllerBase 
    {
        private readonly BillPayManager _repo;

        public BillPayController(BillPayManager repo)
        {
            _repo = repo;
        }
        //Get - Retrieve all billpays
        [HttpGet]
        public IEnumerable<BillPay> Get()
        {
            return _repo.GetAll();
        }

        // GET{value} - Retrieve a specific billpay based on billpay id
        [HttpGet("{id}")]
        public BillPay Get(int id)
        {
            return _repo.Get(id);
        }

        // POST - Adds a new billpay
        [HttpPost]
        public void Post([FromBody] BillPay billpay)
        {
            _repo.Add(billpay);
        }

        // PUT - Update a billpay
        [HttpPut]
        public void Put([FromBody] BillPay billpay)
        {
            _repo.Update(billpay.BillPayID, billpay);
        }

        // DELETE - Deletes a billpay based on billpay id
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
