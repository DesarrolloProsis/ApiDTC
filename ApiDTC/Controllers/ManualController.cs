using ApiDTC.Data;
using ApiDTC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManualController : ControllerBase
    {
        //private readonly PdfConsultasDb _db;

        //private readonly ApiLogger _apiLogger;

        //private readonly string _disk;

        //private readonly string _folder;


        //public ManualController(PdfConsultasDb db, IConfiguration configuration)
        //{
        //    this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
        //    this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
        //    this._db = db ?? throw new ArgumentNullException(nameof(db));
        //    _apiLogger = new ApiLogger();
        //}

        //[HttpGet("{clavePlaza}/{refNum}/{adminId}")]
        //public IActionResult GetPDF(string clavePlaza, string refNum, int adminId)
        //{


        //        //0 = Nuevo, 1 = Firmado, 2 = Almacén
        //      //  PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
        //        var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 0);
        //        return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            
        //}
    }
}
