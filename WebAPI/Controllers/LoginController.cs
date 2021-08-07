using System;
using System.Collections.Generic;
using WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.DataManger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LoginManager _repo;

        public LoginController(LoginManager repo)
        {
            _repo = repo;
        }

        // Get - Get all login entries
        [HttpGet]
        public IEnumerable<Login> Get()
        {
            return _repo.GetAll();
        }

        // GET{value} - retrieve a specific login based on login ID
        [HttpGet("{id}")]
        public Login Get(string id)
        {
            return _repo.Get(id);
        }

        // POST - Add new login information
        [HttpPost]
        public void Post([FromBody] Login login)
        {
            _repo.Add(login);
        }

        // PUT - update existing login information
        [HttpPut]
        public void Put([FromBody] Login login)
        {
            _repo.Update(login.LoginID, login);
        }

        // DELETE - delete a login
        [HttpDelete("{id}")]
        public long Delete(string id)
        {
            return _repo.Delete(id);
        }
    }
}
