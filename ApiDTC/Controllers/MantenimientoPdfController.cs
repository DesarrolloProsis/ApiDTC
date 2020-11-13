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
        [HttpGet("Semanal")]
        public IActionResult GetMantenimientoSemanal()
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), "1234", 1);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Mensual")]
        public IActionResult GetMantenimientoMensual()
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), "1234", 2);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Bimestral")]
        public IActionResult GetMantenimientoBimestral()
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), "1234", 3);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Semestral")]
        public IActionResult GetMantenimientoSemestral()
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), "1234", 4);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("Anual")]
        public IActionResult GetMantenimientoAnual()
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), "1234", 5);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }
    }
}