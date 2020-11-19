namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class CalendarioController : ControllerBase
    {

        #region Attributes
        private readonly CalendarioDb _db;
        #endregion

        #region Constructor
  
        public CalendarioController(CalendarioDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        [HttpGet("Mantenimiento/{month}/{year}")]
        public IActionResult GetCalendarioMantenimiento(int month, int year)
        {
            var dataSet = _db.GetStorePdf(month, year);
            CalendarioPdfCreation pdf = new CalendarioPdfCreation(dataSet.Tables[1], dataSet.Tables[0], "1234", new ApiLogger(), month, year);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpPost("Actividad")]
        public ActionResult<Response> Post([FromBody] ActividadCalendario actividad)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertActivity(actividad);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
    }
}