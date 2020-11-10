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
        [HttpGet("Mantenimiento")]
        public IActionResult GetMantenimiento()
        {
            MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(new ApiLogger(), "1234");
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }
    }
}