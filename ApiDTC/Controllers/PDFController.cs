using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using ApiDTC.Data;
using System.Data;

using System.Reflection;
using System.Globalization;
using ApiDTC.Services;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {

        private readonly PdfConsultasDb _db;
        public PDFController(PdfConsultasDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        // GET: api/PDF
        [HttpGet("{refNum}")]
        public IActionResult GetPDF(string refNum)
        {
            //TODO If getstore is null on
            var get = _db.SearchReference(refNum);
            if(get.Result == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDF(refNum);
                PdfCreation pdf = new PdfCreation(dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf();
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

 
    }


}
