using ApiDTC.Data;
using ApiDTC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnexoDTCController : ControllerBase
    {
        private readonly AnexoDtcDb _db;
        public AnexoDTCController(AnexoDtcDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        [HttpGet("{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> GetComponent(string clavePlaza, string referenceNumber)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetComponentAnexo(clavePlaza, referenceNumber);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
    }
}
