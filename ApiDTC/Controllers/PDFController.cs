namespace ApiDTC.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using ApiDTC.Services;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Http;

    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        #region Attributes
        private readonly PdfConsultasDb _db;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public PDFController(PdfConsultasDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion

        // GET: api/PDF
        [HttpGet("{clavePlaza}/{refNum}/{inicialRef}")]
        public IActionResult GetPDF(string clavePlaza, string refNum, string inicialRef)
        {
            //TODO If getstore is null on
            var get = _db.SearchReference(clavePlaza, refNum);
            if(get.Result == null)
                return NotFound(get);
            else
            {
                
                var dataSet = _db.GetStorePDF(clavePlaza, refNum, inicialRef);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf(0);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        [HttpPut("TerminarReporte/{clavePlaza}/{refNum}/{inicialRef}")]
        public IActionResult TerminarReporte(string clavePlaza, string refNum, string inicialRef)
        {
            //TODO If getstore is null on
            var get = _db.TerminarReporte(clavePlaza, refNum);
            if (get.SqlResult == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDF(clavePlaza, refNum, inicialRef);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                var pdfResult = pdf.NewPdf(1);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        [HttpGet("ReporteAlmacen/{clavePlaza}/{refNum}/{inicialRef}")]
        public IActionResult ReporteAlmacen(string clavePlaza, string refNum, string inicialRef)
        {
            //TODO If getstore is null on
            var get = _db.SearchReference(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDF(clavePlaza, refNum, inicialRef);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf(2);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        [HttpGet("open/{clavePlaza}/{refNum}")]
        public IActionResult GetPDFOpen(string clavePlaza, string refNum)
        {
            //TODO If getstore is null on
            var get = _db.SearchReference(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDFOpen(clavePlaza, refNum);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdfOpen retorna tabla vacía");
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf(0);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        [HttpPost("PdfSellado/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> PdfSellado(string clavePlaza, [FromForm(Name = "file")] IFormFile file, string referenceNumber)
        {
            if(file.Length > 0 || file != null)
            {
                string path = $@"C:\Bitacora\{clavePlaza}\DTC\{referenceNumber}", filename;
                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    filename = $"ReporteDTC-{referenceNumber}-Sellado.pdf";
                    if(System.IO.File.Exists(Path.Combine(path, filename)))
                        System.IO.File.Delete(Path.Combine(path, filename));
                    var fs = new FileStream(Path.Combine(path, filename), FileMode.Create);
                    file.CopyTo(fs);
                    fs.Close();
                    return Ok(path);
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "PDFController: PdfSellado", 2);
                    return NotFound(ex.ToString());
                }
            }
            return NotFound();
        }

        [HttpGet("PdfSellado/{clavePlaza}/{referenceNumber}")]
        public IActionResult GetPdfSellado(string clavePlaza, string referenceNumber)
        {
            string path = $@"C:\Bitacora\{clavePlaza}\DTC\{referenceNumber}\ReporteDTC-{referenceNumber}-Sellado.pdf";
            try
            {
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                return File(new FileStream(path, FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "PDFController: GetPdfSellado", 2);
                return NotFound(ex.ToString());
            }
        }
    }
}
