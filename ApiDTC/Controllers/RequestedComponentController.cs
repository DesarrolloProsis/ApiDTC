using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDTC.Data;
using ApiDTC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestedComponentController : ControllerBase
    {
        private readonly RequestedComponentDb _db;
        public RequestedComponentController(RequestedComponentDb db)
        {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }

        // GET: api/RequestedComponent
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/RequestedComponent/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/RequestedComponent
        [HttpPost]
        public object Post([FromBody] List<RequestedComponent> requestedComponent)
        {
            return _db.PostRequestedComponent(requestedComponent);
            //return new object { };
        }

        // PUT: api/RequestedComponent/5
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
