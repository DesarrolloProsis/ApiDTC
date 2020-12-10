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
        //[HttpGet("Semanal/{plaza}/{carril}")]
        [HttpGet("Semanal/{clavePlaza}/{plaza}/{carril}")]
        public IActionResult GetMantenimientoSemanal(string clavePlaza, string plaza, string carril)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), plaza, 1, carril);
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

        //[HttpGet("Mensual/{plaza}/{carril}")]
        [HttpGet("Mensual/{clavePlaza}/{plaza}/{carril}")]
        public IActionResult GetMantenimientoMensual(string clavePlaza, string plaza, string carril)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), plaza, 2, carril);
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

        //[HttpGet("Trimestral/{plaza}/{carril}")]
        [HttpGet("Trimestral/{clavePlaza}/{plaza}/{carril}")]
        public IActionResult GetMantenimientoTrimestral(string clavePlaza, string plaza, string carril)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), plaza, 3, carril);
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

        //[HttpGet("Semestral/{plaza}/{carril}")]
        [HttpGet("Semestral/{clavePlaza}/{plaza}/{carril}")]
        public IActionResult GetMantenimientoSemestral(string clavePlaza, string plaza, string carril)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), plaza, 4, carril);
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

        //[HttpGet("Anual/{plaza}/{carril}")]
        [HttpGet("Anual/{clavePlaza}/{plaza}/{carril}")]
        public IActionResult GetMantenimientoAnual(string clavePlaza, string plaza, string carril)
        {
            try
            {
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, new ApiLogger(), plaza, 5, carril);
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