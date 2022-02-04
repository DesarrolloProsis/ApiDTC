using ApiDTC.Data;
using ApiDTC.Models;
using ApiDTC.Models.AnexoDTC;
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
        
        [HttpGet("Componentes/{clavePlaza}/{referenceNumber}")]
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

        [HttpGet("Historico/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> GetHistoricoAnexo(string clavePlaza, string referenceNumber)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetHistoricoAnexo(clavePlaza, referenceNumber);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("HistoricoComponetes/{clavePlaza}/{referenceAnexo}")]
        public ActionResult<Response> GetHistoricoComponetesAnexo(string clavePlaza, string referenceAnexo)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetHistoricoComponetesAnexo(clavePlaza, referenceAnexo);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("{clavePlaza}/{editAnexoVersion}")]
        public ActionResult<Response> InsertAnexo(string clavePlaza, bool editAnexoVersion, [FromBody] AnexoDTCInsert insetAnexo)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertAnexoDTC(clavePlaza, insetAnexo);
                if (get.Message == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        
    }
}
