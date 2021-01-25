namespace ApiDTC.Controllers
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
    public class ReporteFotograficoController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;

        private ReporteFotograficoDB _db;
        #endregion

        #region Constructor
        public ReporteFotograficoController(ReporteFotograficoDB db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db)); 
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        [HttpGet("Reporte/{clavePlaza}/{referenceNumber}")]
        public IActionResult GetReporteFotografico(string clavePlaza, string referenceNumber)
        {
            try
            {
                var dataSet = _db.GetStorePDF(clavePlaza, referenceNumber);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStoredPdf retorna tabla vacía");
                ReporteFotograficoPdfCreation pdf = new ReporteFotograficoPdfCreation(clavePlaza, dataSet.Tables[0], new ApiLogger(), 1, referenceNumber);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteFotografico", 2);
                return NotFound(ex.ToString());
            }
            
        }

        [HttpGet("Nuevo/{clavePlaza}/{ubicacion}/{referenceNumber}")]
        public IActionResult GetReporteEqupoNuevo(string clavePlaza, string ubicacion, string referenceNumber)
        {
            try
            {
                var dataSet = _db.GetStorePDF(clavePlaza, referenceNumber);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStoredPdf retorna tabla vacía");
                ReporteFotograficoPdfCreation pdf = new ReporteFotograficoPdfCreation(clavePlaza, dataSet.Tables[0], new ApiLogger(), 2, referenceNumber);
                var pdfResult = pdf.NewPdf();
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

        [HttpGet("Dañado/{clavePlaza}/{ubicacion}/{referenceNumber}")]
        public IActionResult GetReporteEquipoDañado(string clavePlaza, string ubicacion, string referenceNumber)
        {
            try
            {
                var dataSet = _db.GetStorePDF(clavePlaza, referenceNumber);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStoredPdf retorna tabla vacía");
                ReporteFotograficoPdfCreation pdf = new ReporteFotograficoPdfCreation(clavePlaza, dataSet.Tables[0], new ApiLogger(), 3, referenceNumber);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteEquipoDañado", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion
    }
}