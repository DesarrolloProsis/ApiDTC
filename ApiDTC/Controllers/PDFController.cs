namespace ApiDTC.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using ApiDTC.Services;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PDFController : ControllerBase
    {
        #region Attributes

        private readonly PdfConsultasDb _db;

        private readonly ApiLogger _apiLogger;

        private readonly string _disk;

        private readonly string _folder;
        private IConfiguration configuration;

        #endregion Attributes

        #region Constructor

        public PDFController(PdfConsultasDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this.configuration = configuration;
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }

        #endregion Constructor

        [HttpGet("{clavePlaza}/{refNum}/{adminId}")]
        [AllowAnonymous]
        public IActionResult GetPDF(string clavePlaza, string refNum, int adminId)
        {
            //TODO If getstore is null on
            var get = _db.SearchReference(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDF(clavePlaza, refNum, adminId);
                if (dataSet.Tables[1].Rows[0]["ReferenceNumber"].ToString() == "EZ-21203")
                    dataSet.Tables[3].Rows[0]["Modelo"] = "ST1000VX0008";
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 0, configuration);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        //Temporal para dtc con metraje(Alex) push
        [HttpGet("Metros/{clavePlaza}/{refNum}/{inicialRef}")]
        public IActionResult GetPDFMetraje(string clavePlaza, string refNum, string inicialRef)
        {
            //TODO If getstore is null on
            var get = _db.SearchReference(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDFMetraje(clavePlaza, refNum, inicialRef);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 0, configuration);
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
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 0, configuration);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        [HttpPost("PdfSellado/{clavePlaza}/{referenceNumber}/{bandera}")]
        public ActionResult<Response> PdfSellado(string clavePlaza, [FromForm(Name = "file")] IFormFile file, string referenceNumber, bool bandera)
        {
            if (file.Length > 0 || file != null)
            {
                if (file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}", filename;
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        filename = $"DTC-{referenceNumber}-Sellado.pdf";
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        var fs = new FileStream(Path.Combine(path, filename), FileMode.Create);
                        file.CopyTo(fs);
                        fs.Close();
                        if (bandera)
                        {
                            var get = _db.SelladoReporte(clavePlaza, referenceNumber);
                            if (get.SqlResult == null)
                                return NotFound(get);
                            return Ok(path);
                        }
                        else
                            return Ok(Path.Combine(path, filename));
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "PDFController: PdfSellado", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        [HttpGet("PdfExists/{clavePlaza}/{referenceNumber}")]
        public ActionResult PdfExists(string clavePlaza, string referenceNumber)
        {
            string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}\DTC-{referenceNumber}-Sellado.pdf";
            if (System.IO.File.Exists((path)))
                return Ok();
            return NotFound();
        }

        [HttpGet("ActualizarDtc/{clavePlaza}/{referenceNumber}/{status}")]
        public ActionResult<Response> ActualizarDtc(string clavePlaza, string referenceNumber, int status)
        {
            var get = _db.UpdateStatus(clavePlaza, referenceNumber, status);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpPost("ActualizarDtcAdministratores/{clavePlaza}")]
        public ActionResult<Response> ActualizarDtcAdministradores(string clavePlaza, [FromBody] DtcStatusLog dtcStatusLog)
        {
            if (ModelState.IsValid)
            {
                var get = _db.UpdateStatusAdmin(clavePlaza, dtcStatusLog);
                if (get.Result == null)
                    return NotFound(get);
                return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("Autorizado/{clavePlaza}/{referenceNumber}/{UserId}")]
        public ActionResult<Response> PdfAutorizado(string clavePlaza, string referenceNumber, int UserId)
        {
            var get = _db.AutorizadoGmmp(clavePlaza, referenceNumber, UserId);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("GetPdfSellado/{clavePlaza}/{referenceNumber}")]
        public IActionResult GetPdfSellado(string clavePlaza, string referenceNumber)
        {
            string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}\DTC-{referenceNumber}-Sellado.pdf";
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

        [HttpGet("RefrescarArchivo/{clavePlaza}/{referenceNumber}/{adminId}")]
        public IActionResult RefrescarArchivo(string clavePlaza, string referenceNumber, int adminId)
        {
            string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}\DTC-{referenceNumber}-Finalizado.pdf";
            try
            {
                if (System.IO.File.Exists(path))
                {
                    var dataSet = _db.GetStorePDF(clavePlaza, referenceNumber, adminId);
                    if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                        return NotFound("GetStorePdf retorna tabla vacía");
                    PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], referenceNumber, new ApiLogger());
                    //0 = Nuevo, 1 = Firmado, 2 = Almacén
                    var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 1, configuration);
                    return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
                }
                return NotFound("No existe el archivo");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "PDFController: GetPdfFirmado", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("GetPdfFirmado/{clavePlaza}/{referenceNumber}/{adminId}")]
        public IActionResult GetPdfFirmado(string clavePlaza, string referenceNumber, int adminId)
        {
            string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}\DTC-{referenceNumber}-Finalizado.pdf";
            try
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var dataSet = _db.GetStorePDF(clavePlaza, referenceNumber, adminId);
                if (dataSet.Tables[1].Rows[0]["ReferenceNumber"].ToString() == "EZ-21203")
                    dataSet.Tables[3].Rows[0]["Modelo"] = "ST1000VX0008";
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], referenceNumber, new ApiLogger());
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 1, configuration);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "PDFController: GetPdfFirmado", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("FirmarReporte/{clavePlaza}/{refNum}/{adminId}")]
        public IActionResult FirmarReporte(string clavePlaza, string refNum, int adminId)
        {
            var get = _db.FirmarReporte(clavePlaza, refNum);
            if (get.SqlResult == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDF(clavePlaza, refNum, adminId);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 1, configuration);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        [HttpGet("ReporteAlmacen/{clavePlaza}/{refNum}/{adminId}")]
        public IActionResult ReporteAlmacen(string clavePlaza, string refNum, int adminId)
        {
            var get = _db.SearchReference(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
            {
                var dataSet = _db.GetStorePDF(clavePlaza, refNum, adminId);
                if (dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0 || dataSet.Tables[3].Rows.Count == 0)
                    return NotFound("GetStorePdf retorna tabla vacía");
                //0 = Nuevo, 1 = Firmado, 2 = Almacén
                PdfCreation pdf = new PdfCreation(clavePlaza, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], dataSet.Tables[3], refNum, new ApiLogger());
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}", 2, configuration);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
        }

        [HttpPost("TablaActEscaneado/{clavePlaza}/{reference}/{Tipo}")]
        public ActionResult<Response> SubirPDFEscaneadoGenerico([FromForm(Name = "file")] IFormFile file, string clavePlaza, string reference, int Tipo)
        {
            /*
             * 1 - TLA-DF-21181-06-Diagnostico
             * 2 - TLA-DF-21181-06-FichaTecnica
             */
            string filename = "";
            if (file.Length > 0 || file != null)
            {
                if (file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reference}\";
                    if (Tipo == 1)
                    {
                        filename = $"{reference}-DiagnosticoSellado.pdf";
                    }
                    else
                    {
                        if (Tipo == 2)
                        {
                            filename = $"{reference}-FichaTecnicaSellado.pdf";
                        }
                    }
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
                        _apiLogger.WriteLog(clavePlaza, ex, "Mantenimiento: SubirPDFEscaneadoGenerico", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        [HttpGet("GetPdfGenerico/{clavePlaza}/{reference}/{Tipo}")]
        public IActionResult GetPdfGenerico(string clavePlaza, string reference, int Tipo)
        {
            /*
             * 1 - TLA-DF-21181-06-Diagnostico
             * 2 - TLA-DF-21181-06-FichaTecnica
             */
            string path = "";
            if (Tipo == 1)
            {
                path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reference}\{reference}-DiagnosticoSellado.pdf";
            }
            else
            {
                if (Tipo == 2)
                {
                    path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reference}\{reference}-FichaTecnicaSellado.pdf";
                }
            }
            try
            {
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                return File(new FileStream(path, FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "PDFController: GetPdfGenerico", 2);
                return NotFound(ex.ToString());
            }
        }
    }
}