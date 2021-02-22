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
    public class FichaTecnicaAtencionController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;

        private readonly FichaTecnicaDb _db;
        #endregion

        #region Constructor
        public FichaTecnicaAtencionController(FichaTecnicaDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        [HttpGet("{clavePlaza}/{ubicacion}/{referenceNumber}")]
        public IActionResult GetFichaTecnicaAtencion(string clavePlaza, string ubicacion, string referenceNumber)
        {
            try
            {
                FichaTecnicaAtencionPdfCreation pdf = new FichaTecnicaAtencionPdfCreation(clavePlaza, new ApiLogger(), ubicacion, referenceNumber);
                var pdfResult = pdf.NewPdf();
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: GetFichaTecnicaAtencion", 2);
                return NotFound(ex.ToString());
            }
            
        }

        [HttpPost("FichaTecnicaDiagnostico/{clavePlaza}")]
        public ActionResult<Response> InsertFichaTecnicaDiagnostico(string clavePlaza, [FromBody] FichaTecnicaDiagnostico fichaTecnicaDiagnostico)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertDiagnosticoFichaTecnica(clavePlaza, fichaTecnicaDiagnostico);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("FichaTecnicaDiagnostico/{clavePlaza}")]
        public ActionResult<Response> InsertFichaTecnicaIntervencion(string clavePlaza, [FromBody] FichaTecnicaIntervencion fichaTecnicaIntervencion)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertFichaTecnicaIntervencion(clavePlaza, fichaTecnicaIntervencion);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        #region FichaTecnicaImages
        [HttpPost("Images/{clavePlaza}/{reportNumber}")]
        public ActionResult<Response> InsertImageNuevo(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string reportNumber)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs";
                string filename;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    if(Directory.GetFiles(dir).Length >= 4)
                        return NotFound("Ya existen cuatro imágenes");
                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{reportNumber}_FichaTecnicaAtencionImgs_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{reportNumber}_FichaTecnicaAtencionImage_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    var fs = new FileStream(Path.Combine(dir, filename), FileMode.Create);
                    image.CopyTo(fs);
                    fs.Close();
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
                return Ok(Path.Combine(dir, filename));
            }
            else
                return NotFound("Insert another image");
        }

        [HttpGet("Images/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadFichaTecnicaImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: DownloadFichaTecnicaImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Images/GetPaths/{clavePlaza}/{reportNumber}")]
        public ActionResult<List<string>> GetImagesFichaTecnica(string clavePlaza, string reportNumber)
        {
            try
            {
                string directoy = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: GetImagesFichaTecnica", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Images/DeleteImg/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<string> DeleteFichaTecnicaImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs").Length == 0)
                    Directory.Delete($@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: DeleteFichaTecnicaImg", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion
        #endregion
    }
}