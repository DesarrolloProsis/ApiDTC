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

        [HttpGet("AnexoA/{clavePlaza}")]
        public IActionResult GetAnexoA(string clavePlaza)
        {
            try
            {

                AnexosPdfCreation pdf = new AnexosPdfCreation(clavePlaza, new ApiLogger());
                var pdfResult = pdf.NewPdfA($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteEqupoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("AnexoB/{clavePlaza}")]
        public IActionResult GetAnexoB(string clavePlaza)
        {
            try
            {

                AnexosPdfCreation pdf = new AnexosPdfCreation(clavePlaza, new ApiLogger());
                var pdfResult = pdf.NewPdfB($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteEqupoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

    }
}
