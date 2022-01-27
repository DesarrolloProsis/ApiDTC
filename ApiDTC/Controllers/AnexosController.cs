﻿using ApiDTC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiDTC.Data;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnexosController : ControllerBase
    {
        #region Attributes
        private readonly InventarioDb _db;

        //private readonly PdfConsultasDb _db;

        private readonly ApiLogger _apiLogger;

        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor

        public AnexosController(InventarioDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion
        [HttpGet("AnexoA/{clavePlaza}")]
        public IActionResult GetInventario(string clavePlaza)
        {
            try
            {

                AnexoAPdfCreation pdf = new AnexoAPdfCreation(clavePlaza, new ApiLogger());
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteEqupoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }
    }
}
