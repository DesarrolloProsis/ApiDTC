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
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CalendarioController : ControllerBase
    {

        #region Attributes
        private readonly CalendarioDb _db;

        private readonly ApiLogger _apiLogger;

        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor
  
        public CalendarioController(CalendarioDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods

        [HttpPost("GetActividadesUsuario")]
        public IActionResult GetActividadesUsuario([FromBody] ActividadesUsuario actividadesUsuario)
        {
            var get = _db.GetActividadesUsuario("USR", actividadesUsuario, this._disk, this._folder);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpPost("GetActividadesFiltroReferencia")]
        public IActionResult GetActividadesFiltroReferencia([FromBody] ActividadesUsuarioFiltro actividadesUsuarioFiltro)
        {
            var get = _db.GetActividadesFiltroReferencia("USR", actividadesUsuarioFiltro);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("Mantenimiento/{clavePlaza}/{month}/{year}/{userId}/{squareId}")]
        public IActionResult GetCalendarioMantenimiento(string clavePlaza, int month, int year, int userId, string squareId)
        {
            var dataSet = _db.GetStorePdf(clavePlaza, month, year, userId, squareId == "1Bi" ? squareId + "s" : squareId);
            if (dataSet.Tables[1].Rows.Count == 0)
                return NotFound();
            CalendarioPdfCreation pdf = new CalendarioPdfCreation(clavePlaza, dataSet.Tables[1], dataSet.Tables[0], clavePlaza, new ApiLogger(), month, year, squareId, userId);
            var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
            return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpPost("CalendarioEscaneado/{clavePlaza}/{month}/{year}/{idUser}")]
        public ActionResult<Response> CalendarioEscaneado([FromForm(Name = "file")] IFormFile file, string clavePlaza, int month, int year, int idUser)
        {
            if(file.Length > 0 || file != null)
            {
                if(file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\CalendariosMantenimiento\{year}\{month}\";
                    string filename = $"{clavePlaza.ToUpper()}{year}-{month.ToString("00")}C-{idUser}-Escaneado.pdf";
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        var fs = new FileStream(Path.Combine(path, filename), FileMode.Create);
                        file.CopyTo(fs);
                        fs.Close();
                        return Ok(path);
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "CalendarioController: CalendarioEscaneado", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        [HttpGet("Exists/{clavePlaza}/{year}/{month}/{idUser}")]
        public ActionResult CalendarioExists(string clavePlaza, int year, int month, int idUser)
        {
            string path =  $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\CalendariosMantenimiento\{year}\{month}\{clavePlaza.ToUpper()}{year}-{month.ToString("00")}C-{idUser}-Escaneado.pdf";
            if(System.IO.File.Exists((path)))
                return Ok();
            return NotFound();
        }

        [HttpGet("GetPdfSellado/{clavePlaza}/{year}/{month}/{idUser}")]
        public ActionResult GetPdfSellado(string clavePlaza, int year, int month, int idUser)
        {            
            
            string path =  $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\CalendariosMantenimiento\{year}\{month}\{clavePlaza.ToUpper()}{year}-{month.ToString("00")}C-{idUser}-Escaneado.pdf";
            try
            {
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                return File(new FileStream(path, FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "CalendarioController: GetPdfSellado", 2);
                return NotFound(ex.ToString());
            }
        }
        
        [HttpDelete("DeleteCalendar/{clavePlaza}/{CalendarId}")]
        public ActionResult<Response> DeleteCalendar(string clavePlaza, int CalendarId)
        {
            var get = _db.DeleteCalendar(clavePlaza, CalendarId);
            if (get.Result == null)
                return BadRequest(get);
            else
                return Ok(get);
        }        
        
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
        public ActionResult<Response> GetActividad(string clavePlaza, [FromBody] ActividadMesYear infoActividad)//
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetActivity(clavePlaza, infoActividad, this._disk, this._folder);
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

        [HttpPost("CalendarReportData/{clavePlaza}/{updateFlag}")]
        public ActionResult<Response> InsertCalendarReportData(string clavePlaza, bool updateFlag, [FromBody] CalendarReportData calendarReportData)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertCalendarReportData(clavePlaza, calendarReportData, updateFlag);
                if (get.Result == null)
                    return NotFound(get); 
                else{
                    return Ok(get);
                }
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


        [HttpGet("GetActividadesPreventivo/{clavePlaza}/{userId}/{squareId}/{year}")]
        public IActionResult getActividadesPreventivo(string clavePlaza, int userId, string squareId, int year )
        {
            var dataSet = _db.getActividadesPreventivo(clavePlaza, userId,   squareId == "1Bi" ? squareId + "s" : squareId, year);
            if (dataSet.Tables[0].Rows.Count == 0)
                return NotFound();
            return Ok(dataSet);
        }
        #endregion
    }
}