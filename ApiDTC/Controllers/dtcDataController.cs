namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
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
        public ActionResult<SqlResult> Get()
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
        public ActionResult<SqlResult> GetDtcData(string refNum)
        {
            //string resultado = _db.GetReferenceNum(refNum);
            SqlResult result = _db.GetReferenceNumber(refNum);
            return result;
        }

        [HttpGet("InvalidReferenceNumbers")]
        public ActionResult<SqlResult> GetInvalidReferenceNumbers()
        {
            //string resultado = _db.GetReferenceNum(refNum);
            SqlResult result = _db.GetInvalidNumbers();
            return result;
        }

        //TODO Ajustar petición POST nuevo DTC
        // POST: api/dtcData
        [HttpPost]
        public SqlResult Post([FromBody] DtcData dtcData)
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
