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

        [HttpGet("Supervisor/{clavePlaza}/{plazaId}")]
        public ActionResult<Response> GetId(string clavePlaza, string plazaId)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetSupervisores(clavePlaza, plazaId);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("Supervisor/{clavePlaza}/{plazaId}")]
        public ActionResult<Response> GetListaSupervisores(string clavePlaza, string plazaId)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetSupervisores(clavePlaza, plazaId);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("Testigos/{clavePlaza}/{plazaId}")]
        public ActionResult<Response> GetListaTestigos(string clavePlaza, string plazaId)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetTestigos(clavePlaza, plazaId);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
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

        [HttpPost("{clavePlaza}/{isSubAnexo}")]
        public ActionResult<Response> InsertAnexo(string clavePlaza, bool isSubAnexo, [FromBody] AnexoDTCInsert insertAnexo)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertAnexoDTC(clavePlaza, isSubAnexo, insertAnexo);
                if (get.Message == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        
    }
}
