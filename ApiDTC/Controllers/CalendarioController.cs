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

        [HttpGet("Mantenimiento")]
        public IActionResult GetCalendarioMantenimiento()
        {
            CalendarioPdfCreation pdf = new CalendarioPdfCreation(new ApiLogger(), "1234");
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
        [HttpPost("ActividadMesYear")]
        public ActionResult<Response> GetActividad([FromBody] ActividadMesYear infoActividad)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetActivity(infoActividad);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
    }
}