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
    public class SquaresCatalogController : ControllerBase
    {
        private readonly SquaresCatalogDb _db;
        public SquaresCatalogController(SquaresCatalogDb db)
        {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }

        // GET: api/SquaresCatalog
        [HttpGet]
        public ActionResult<OperationResult> Get()
        {
            return _db.GetSquaresCatalog();
        }

        // GET: api/SquaresCatalog/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/SquaresCatalog
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/SquaresCatalog/5
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
