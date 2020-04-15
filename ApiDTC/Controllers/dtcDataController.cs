﻿namespace ApiDTC.Controllers
{
    using System;
    using System.Threading.Tasks;
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
        public ActionResult<Response> Get()
        {   
            var get = _db.GetDTC();
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("{refNum}")]
        public ActionResult<Response> GetDtcData(string refNum)
        {
            var get = _db.GetReferenceNumber(refNum.Substring(0, 9));
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("InvalidReferenceNumbers")]
        public ActionResult<Response> GetInvalidReferenceNumbers()
        {
            var get = _db.GetInvalidNumbers();
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //TODO Ajustar petición POST nuevo DTC
        // POST: api/dtcData/NuevoDtc
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
