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
    public class TypeDescriptionsController : ControllerBase
    {
        private readonly TypeDescriptionsDb _db;
        public TypeDescriptionsController(TypeDescriptionsDb db)
        {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }

        // GET: api/TypeDescriptions
        [HttpGet]
        public ActionResult<SqlResult> Get()
        {
            return _db.GetTypeDescriptionsData();
        }

        // GET: api/TypeDescriptions/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TypeDescriptions
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/TypeDescriptions/5
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
