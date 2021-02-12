﻿namespace ApiDTC.Controllers
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

        #region Methods
        [HttpGet("Mantenimiento/{clavePlaza}/{month}/{year}/{userId}/{squareId}")]
        public IActionResult GetCalendarioMantenimiento(string clavePlaza, int month, int year, int userId, string squareId)
        {
            var dataSet = _db.GetStorePdf(clavePlaza, month, year, userId, squareId == "1Bi" ? squareId + "s" : squareId);
            if (dataSet.Tables[1].Rows.Count == 0)
                return NotFound();
            CalendarioPdfCreation pdf = new CalendarioPdfCreation(clavePlaza, dataSet.Tables[1], dataSet.Tables[0], clavePlaza, new ApiLogger(), month, year, squareId);
            var pdfResult = pdf.NewPdf();
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }
        //[HttpDelete("DeleteCalendar/{month}/{year}/{userId}/{squareId}")]
        [HttpDelete("DeleteCalendar/{clavePlaza}/{CalendarId}")]
        public ActionResult<Response> DeleteCalendar(string clavePlaza, int CalendarId)
        {
            var get = _db.DeleteCalendar(clavePlaza, CalendarId);
            if (get.Result == null)
                return BadRequest(get);
            else
                return Ok(get);
        }        
        //[HttpPost("Actividad")]
        [HttpPost("Actividad/{clavePlaza}")]
        public ActionResult<Response> Post(string clavePlaza, [FromBody] ActividadCalendario actividad)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertActivity(clavePlaza, actividad);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
        //[HttpPost("ObservacionesInsert")]
        [HttpPost("ObservacionesInsert/{clavePlaza}")]
        public ActionResult<Response> ObservacionesInsert(string clavePlaza, [FromBody] InsertCommentCalendar actividad)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertComent(clavePlaza, actividad);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("getComentario/{clavePlaza}")]
        public ActionResult<Response> GetComentario(string clavePlaza, [FromBody] ActividadMesYear infoActividad)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetStoreFrontComment(clavePlaza, infoActividad);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("ActividadMesYear/{clavePlaza}")]
        public ActionResult<Response> GetActividad(string clavePlaza, [FromBody] ActividadMesYear infoActividad)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetActivity(clavePlaza, infoActividad);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("Actividades/{clavePlaza}/{roll}/{frequency}")]
        public ActionResult<Response> GetActivities(string clavePlaza, int roll, int frequency)
        {
            var get = _db.GetActivities(clavePlaza, roll, frequency);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpPost("CalendarReportActivities/{clavePlaza}/{calendarId}")]
        public ActionResult<Response> InsertCalendarReportActivities(string clavePlaza, int CalendarId, [FromBody] List<CalendarActivity> calendarActivities)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertCalendarReportActivities(clavePlaza.ToUpper(), CalendarId, calendarActivities);
                if (get.Result == null)
                    return NotFound(get);
                return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("CalendarReportData/{clavePlaza}")]
        public ActionResult<Response> InsertCalendarReportData(string clavePlaza, [FromBody] CalendarReportData calendarReportData)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertCalendarReportData(clavePlaza, calendarReportData);
                if (get.Result == null)
                    return NotFound(get);
                return Ok(get);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("CalendarioReportDataEdit/{clavePlaza}/{calendarId}")]
        public ActionResult<Response> GetReportDataCalendarioEdit(string clavePlaza, int calendarId)
        {            
            
                var get = _db.GetDataReportEdit(clavePlaza, calendarId);
                if (get.Result == null)
                    return NotFound(get);
                return Ok(get);
          
        }

        [HttpPost("CalendarDateLog/{clavePlaza}")]
        public ActionResult<Response> InsertCalendarDateLog(string clavePlaza, [FromBody] CalendarDateLog calendarDateLog)
        {
            if(ModelState.IsValid)
            {
                var get = _db.InsertCalendarDateLog(clavePlaza, calendarDateLog);
                if(get.Result == null)
                    return NotFound(get);
                return Ok(get);
            }
            return BadRequest(ModelState);
        }
        
        [HttpGet("CalendarInfo/{clavePlaza}/{calendarId}")]
        public ActionResult<Response> GetCalendarInfo(string clavePlaza, int calendarId)
        {
            var get = _db.GetCalendarInfo(clavePlaza, calendarId);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }
        #endregion
    }
}