﻿namespace ApiDTC.Controllers
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
        [HttpGet("{tipo}/{clavePlaza}/{noReporte}")]
        public IActionResult GetMantenimiento(int tipo, string clavePlaza, string noReporte)
        {
            try
            {
                var get = _db.GetStorePDF(clavePlaza, noReporte);
                if (get.Tables[0].Rows.Count == 0 || get.Tables[1].Rows.Count == 0)
                    return NotFound();
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, get.Tables[0], get.Tables[1], new ApiLogger(), tipo, noReporte);
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
        #endregion
    }
}