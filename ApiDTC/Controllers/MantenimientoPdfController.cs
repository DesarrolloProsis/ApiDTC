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
        [HttpGet("Semanal/{plaza}/{carril}")]
        public IActionResult GetMantenimientoSemanal(string plaza, string carril)
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), plaza, 1, carril);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Mensual/{plaza}/{carril}")]
        public IActionResult GetMantenimientoMensual(string plaza, string carril)
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), plaza, 2, carril);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Trimestral/{plaza}/{carril}")]
        public IActionResult GetMantenimientoTrimestral(string plaza, string carril)
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), plaza, 3, carril);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Semestral/{plaza}/{carril}")]
        public IActionResult GetMantenimientoSemestral(string plaza, string carril)
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), plaza, 4, carril);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Anual/{plaza}/{carril}")]
        public IActionResult GetMantenimientoAnual(string plaza, string carril)
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), plaza, 5, carril);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }
    }
}