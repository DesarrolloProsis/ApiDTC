namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiDTC.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class FichaTecnicaAtencionController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public FichaTecnicaAtencionController()
        {
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        [HttpGet("{clavePlaza}/{ubicacion}")]
        public IActionResult GetFichaTecnicaAtencion(string clavePlaza, string ubicacion)
        {
            try
            {
                FichaTecnicaAtencionPdfCreation pdf = new FichaTecnicaAtencionPdfCreation(clavePlaza, new ApiLogger(), ubicacion, string.Empty);
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

        [HttpPost("FichaTecnicaAtencion/Images/{clavePlaza}/{reportNumber}")]
        public ActionResult<Response> InsertImageNuevo(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string reportNumber)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs";
                string filename;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{reportNumber}_DiagnosticoFallaImgs_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{reportNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    var fs = new FileStream(Path.Combine(dir, filename), FileMode.Create);
                    image.CopyTo(fs);
                    fs.Close();
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
                return Ok(Path.Combine(dir, filename));
            }
            else
                return NotFound("Insert another image");
        }

        [HttpGet("EquipoNuevo/Images/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadEquipoNuevoImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: DownloadEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("EquipoNuevo/Images/GetPaths/{clavePlaza}/{reportNumber}")]
        public ActionResult<List<string>> GetImagesEquipoNuevo(string clavePlaza, string reportNumber)
        {
            try
            {
                string directoy = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: GetImagesEquipoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("EquipoNuevo/Images/DeleteImg/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<string> DeleteEquipoNuevoImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs").Length == 0)
                    Directory.Delete($@"C:\Bitacora\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: DeleteEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion
    }
}