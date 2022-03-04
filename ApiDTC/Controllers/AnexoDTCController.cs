using ApiDTC.Data;
using ApiDTC.Models;
using ApiDTC.Services;
using ApiDTC.Models.AnexoDTC;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnexoDTCController : ControllerBase
    {
        #region Atributos
        private readonly AnexoDtcDb _db;

        private readonly string _disk;

        private readonly string _folder;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructores
        public AnexoDTCController(AnexoDtcDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }

        #endregion

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

        [HttpGet("Historico/{clavePlaza}/{referenceDTC}")]
        public ActionResult<Response> GetHistoricoAnexo(string clavePlaza, string referenceDTC)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetHistoricoAnexo(clavePlaza, referenceDTC);
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

        [HttpGet("AnexoA/{clavePlaza}/{referenciaAnexo}")]
        public IActionResult GetAnexoA(string clavePlaza, string referenciaAnexo)
        {
            try
            {
                var dataSet = _db.GetAnexoPDF(referenciaAnexo);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                AnexosPdfCreation pdf = new AnexosPdfCreation(clavePlaza, referenciaAnexo, dataSet.Tables[0], dataSet.Tables[1], new ApiLogger());
                var pdfResult = pdf.NewPdfA($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(referenciaAnexo, ex, "ReporteFotograficoController: GetReporteEqupoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("AnexoB/{clavePlaza}/{referenciaAnexo}")]
        public IActionResult GetAnexoB(string clavePlaza, string referenciaAnexo)
        {
            try
            {
                var dataSet = _db.GetAnexoPDF(referenciaAnexo);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                AnexosPdfCreation pdf = new AnexosPdfCreation(clavePlaza, referenciaAnexo, dataSet.Tables[0], dataSet.Tables[1], new ApiLogger());
                var pdfResult = pdf.NewPdfB($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(referenciaAnexo, ex, "ReporteFotograficoController: GetReporteEqupoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

    }
}
