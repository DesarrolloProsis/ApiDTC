namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiDTC.Data;
    using ApiDTC.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class MantenimientoPdfController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;

        private readonly MantenimientoPdfDb _db;
        #endregion

        #region Constructor
        public MantenimientoPdfController(MantenimientoPdfDb db)
        {
            this._db = db ?? throw new ArgumentException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        [HttpGet("Semanal/{clavePlaza}/{noReporte}")]
        public IActionResult GetMantenimientoSemanal(string clavePlaza, string noReporte)
        {
            try
            {
                var get = _db.GetStorePDF(clavePlaza, noReporte);
                if (get.Tables[0].Rows.Count == 0 || get.Tables[1].Rows.Count == 0)
                    return NotFound();
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, get.Tables[0], get.Tables[1], new ApiLogger(), 1, noReporte);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoSemanal", 2);
                return NotFound(ex.ToString());
            }
            
        }
        [HttpGet("Semanal/Carril/{clavePlaza}/{noReporte}")]
        public IActionResult GetMantenimientoCarrilSemanal(string clavePlaza, string noReporte)
        {
            try
            {
                var get = _db.GetStorePDF(clavePlaza, noReporte);
                if (get.Tables[0].Rows.Count == 0 || get.Tables[1].Rows.Count == 0)
                    return NotFound();
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, get.Tables[0], get.Tables[1], new ApiLogger(), 6, noReporte);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoSemanal", 2);
                return NotFound(ex.ToString());
            }
        }

        //[HttpGet("Mensual/{plaza}/{ubicacion}")]
        /*[HttpGet("Mensual/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoMensual(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 2, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoMensual", 2);
                return NotFound(ex.ToString());
            }
        }

        //[HttpGet("Trimestral/{plaza}/{ubicacion}")]
        [HttpGet("Trimestral/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoTrimestral(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 3, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoTrimestral", 2);
                return NotFound(ex.ToString());
            }
        }

        //[HttpGet("Semestral/{plaza}/{ubicacion}")]
        [HttpGet("Semestral/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoSemestral(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 4, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoSemestral", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Anual/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoAnual(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 5, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoAnual", 2);
                return NotFound(ex.ToString());
            }
        }

        

        [HttpGet("Mensual/Carril/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoCarrilMensual(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 7, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoCarrilMensual", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Trimestral/Carril/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoCarrilTrimestral(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 8, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoCarrilTrimestral", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Semestral/Carril/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoCarrilSemestral(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 9, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoCarrilSemestral", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Anual/Carril/{clavePlaza}/{plaza}/{ubicacion}/{inicio}")]
        public IActionResult GetMantenimientoCarrilAnual(string clavePlaza, string plaza, string ubicacion, int inicio)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 10, ubicacion, string.Empty);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimientoCarrilAnual", 2);
                return NotFound(ex.ToString());
            }
        }*/
        #endregion
    }
}