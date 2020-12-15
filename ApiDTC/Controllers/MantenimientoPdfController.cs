namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiDTC.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class MantenimientoPdfController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public MantenimientoPdfController()
        {
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        //[HttpGet("Semanal/{plaza}/{ubicacion}")]
        [HttpGet("Semanal/{clavePlaza}/{plaza}/{ubicacion}")]
        public IActionResult GetMantenimientoSemanal(string clavePlaza, string plaza, string ubicacion)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 1, ubicacion, string.Empty);
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
        [HttpGet("Mensual/{clavePlaza}/{plaza}/{ubicacion}")]
        public IActionResult GetMantenimientoMensual(string clavePlaza, string plaza, string ubicacion)
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
        [HttpGet("Trimestral/{clavePlaza}/{plaza}/{ubicacion}")]
        public IActionResult GetMantenimientoTrimestral(string clavePlaza, string plaza, string ubicacion)
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
        [HttpGet("Semestral/{clavePlaza}/{plaza}/{ubicacion}")]
        public IActionResult GetMantenimientoSemestral(string clavePlaza, string plaza, string ubicacion)
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

        //[HttpGet("Anual/{plaza}/{ubicacion}")]
        [HttpGet("Anual/{clavePlaza}/{plaza}/{ubicacion}")]
        public IActionResult GetMantenimientoAnual(string clavePlaza, string plaza, string ubicacion)
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

        //[HttpGet("Anual/{plaza}/{ubicacion}")]
        [HttpGet("Dañado/{clavePlaza}/{plaza}/{ubicacion}/{referenceNumber}")]
        public IActionResult GetEquipoDaniado(string clavePlaza, string plaza, string ubicacion, string referenceNumber)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), 6, ubicacion, referenceNumber);
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
        #endregion
    }
}