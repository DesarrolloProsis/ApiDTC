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
    public class dtcDataController : ControllerBase
    {
        private readonly dtcDataDb _db;
        public dtcDataController(dtcDataDb db)
        {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }

        //GET: api/dtcData
       [HttpGet]
        public ActionResult<List<DTCData>> Get()
        {
            return _db.GetDTC();
        }

        // GET: api/dtcData/5
        //[HttpGet("{refNum}")]
        //public string<dtcData> Get(string refNum)
        //{
        //    return "value";
        //}

        [HttpGet("{refNum}")]
        public IActionResult GetdtcData(string refNum)
        {
            string resultado = _db.GetReferenceNum(refNum);
            return Ok(resultado);
        }

        // POST: api/dtcData
        [HttpPost]
        public bool Post([FromBody] DTCData dtcData)
        {
             return _db.GetStoredtcData(dtcData);

        }

        // PUT: api/dtcData/5
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
