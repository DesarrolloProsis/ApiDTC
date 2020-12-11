namespace ApiDTC.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using ApiDTC.Services;
    using ApiDTC.Data;

    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        #region Attributes
        private readonly PdfConsultasDb _db;
        #endregion

        #region Constructor
        public PDFController(PdfConsultasDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        // GET: api/PDF
        //[HttpGet("{refNum}/{inicialRef}")]
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
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf();
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        //[HttpGet("open/{refNum}")]
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
                var pdfResult = pdf.NewPdf();
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }
    }
}
