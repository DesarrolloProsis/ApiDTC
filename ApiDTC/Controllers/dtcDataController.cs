namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class DtcDataController : ControllerBase
    {
        private readonly DtcDataDb _db;
        public DtcDataController(DtcDataDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }

        //GET: api/dtcData
        [HttpGet]
        public IActionResult Get()
        {   
            var get = _db.GetDTC();
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        // GET: api/dtcData/5
        //[HttpGet("{refNum}")]
        //public string<dtcData> Get(string refNum)
        //{
        //    return "value";
        //}

        [HttpGet("{refNum}")]
        public IActionResult GetDtcData(string refNum)
        {
            var get = _db.GetReferenceNumber(refNum);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("InvalidReferenceNumbers")]
        public IActionResult GetInvalidReferenceNumbers()
        {
            var get = _db.GetInvalidNumbers();
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //TODO Ajustar petición POST nuevo DTC
        // POST: api/dtcData
        [HttpPost]
        public Response Post([FromBody] DtcData dtcData)
        {
             return _db.GetStoredDtcData(dtcData);
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
