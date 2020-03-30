using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDTC.Data;
using ApiDTC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly LoginDB _db;
        public LoginController(LoginDB db) {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }

        // GET: api/Login
        [HttpGet("{userName}/{passWord}/{flag}")]
        public IEnumerable<Login> GetLogin(string userName, string passWord, bool flag)
        {                        
               return _db.GetStoreLogin(userName, passWord, flag);


        }
        // GET: api/Login
        [HttpGet("ValidUser/{userName}/{passWord}/{flag}")]
        public IEnumerable<Cokie> GetCokie(string userName, string passWord, bool flag)
        {
                                      
                return _db.GetStoreLoginCokie(userName, passWord, flag);

        }
        // GET: api/Login
        [HttpGet("buscarTec/{numPlaza}")]
        public List<SelectListItem> GetCokie(string numPlaza)
        {

            return _db.GetTec(numPlaza);
                

        }
        // GET: api/Login
        [HttpGet("buscarHeaderTec/{idTec}")]
        public IEnumerable<Login> GetCokie(int idTec)
        {

            return _db.GetHeadTec(idTec);


        }
        // POST: api/Login
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Login/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
