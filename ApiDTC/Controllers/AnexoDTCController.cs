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
        [HttpPost("Supervisor/{clavePlaza}")]
        public ActionResult<Response> InsertSupervisor(string clavePlaza, [FromBody] InsertUsuarioAnexo insertSupervision)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertSupervisor(clavePlaza, insertSupervision);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Testigos/{clavePlaza}")]
        public ActionResult<Response> InsertTestigo(string clavePlaza, [FromBody] InsertUsuarioAnexo insertTestigos)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertTestigo(clavePlaza, insertTestigos);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("ComponentesRequest/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> GetComponentRequested(string clavePlaza, string referenceNumber)
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

        [HttpGet("Historico/{clavePlaza}")]
        public ActionResult<Response> GetHistoricoAnexo(string clavePlaza)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetHistoricoAnexo(clavePlaza);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("HistoricoComponetesAnexo/{clavePlaza}/{referenceAnexo}")]
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

        [HttpGet("HeaderAnexo/{clavePlaza}/{referenceAnexo}")]
        public ActionResult<Response> GetHeaderAnexo(string clavePlaza, string referenceAnexo)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetHeaderAnexo(clavePlaza, referenceAnexo);
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
