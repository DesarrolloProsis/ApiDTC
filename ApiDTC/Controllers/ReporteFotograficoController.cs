namespace ApiDTC.Controllers
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

        #region Reporte fotográfico mantenimiento preventivo
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

        [HttpPost("MantenimientoPreventivo/Images/{clavePlaza}/{reportNumber}/{semana}")]
        public ActionResult<Response> InsertImageNuevo(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string reportNumber, int semana)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\MantenimientoPreventivoImgs\{semana}";
                string filename;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{reportNumber}_MantenimientoPreventivoImgs_{semana}_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{reportNumber}_MantenimientoPreventivoImgs_{semana}_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    var fs = new FileStream(Path.Combine(dir, filename), FileMode.Create);
                    image.CopyTo(fs);
                    fs.Close();
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
                return Ok(Path.Combine(dir, filename));
            }
            else
                return NotFound("Insert another image");
        }

        [HttpGet("MantenimientoPreventivo/Images/{clavePlaza}/{reportNumber}/{fileName}/{semana}")]
        public ActionResult<DtcImage> DownloadEquipoNuevoImg(string clavePlaza, string reportNumber, string fileName, int semana)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\MantenimientoPreventivoImgs\{semana}\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: DownloadEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("MantenimientoPreventivo/Images/GetPaths/{clavePlaza}/{reportNumber}/{semana}")]
        public ActionResult<List<string>> GetImagesEquipoNuevo(string clavePlaza, string reportNumber, int semana)
        {
            try
            {
                string directoy = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\MantenimientoPreventivoImgs\{semana}";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetImagesEquipoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("MantenimientoPreventivo/Images/DeleteImg/{clavePlaza}/{reportNumber}/{fileName}/{semana}")]
        public ActionResult<string> DeleteEquipoNuevoImg(string clavePlaza, string reportNumber, string fileName, int semana)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\MantenimientoPreventivoImgs\{semana}\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\MantenimientoPreventivoImgs\{semana}").Length == 0)
                    Directory.Delete($@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\MantenimientoPreventivoImgs\{semana}");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: DeleteEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion

        #region Equipo nuevo y dañado
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
        
        #endregion
    }
}