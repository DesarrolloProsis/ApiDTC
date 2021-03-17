namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Aspose.Imaging;
    using Aspose.Imaging.ImageOptions;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FichaTecnicaAtencionController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;

        private readonly FichaTecnicaDb _db;

        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor
        public FichaTecnicaAtencionController(FichaTecnicaDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
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
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
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

        
        [HttpGet("FichaTecnicaExists/{clavePlaza}/{referenceNumber}")]
        public ActionResult PdfExists(string clavePlaza, string referenceNumber)
        {
            string path =  $@"{this._disk}:\{this._folder}\{clavePlaza}\Reportes\{referenceNumber}\{referenceNumber}-FichaTecnicaSellado.pdf";
            if(System.IO.File.Exists((path)))
                return Ok();
            return NotFound();
        }

        [HttpPost("FichaTecnicaSellada/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> FichaTecnicaSellada(string clavePlaza, [FromForm(Name = "file")] IFormFile file, string referenceNumber)
        {
            if(file.Length > 0 || file != null)
            {
                if(file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\Reportes\{referenceNumber}", filename;
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        filename = $"{referenceNumber}-FichaTecnicaSellado.pdf";
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        var fs = new FileStream(Path.Combine(path, filename), FileMode.Create);
                        file.CopyTo(fs);
                        fs.Close();
                        return Ok(path);
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: FichaTecnicaSellada", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        [HttpPost("FichaTecnica/{clavePlaza}")]
        public ActionResult<Response> InsertFichaTecnica(string clavePlaza, [FromBody] FichaTecnica fichaTecnica)
        {
            if(ModelState.IsValid)
            {
                var post = _db.InsertFichaTecnica(clavePlaza, fichaTecnica);
                if(post.Result == null)
                    return NotFound(post);
                return Ok(post);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("FichaTecnica/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> GetFichaTecnica(string clavePlaza, string referenceNumber)
        {
            var get = _db.GetTechnicalSheet(clavePlaza, referenceNumber);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpPost("FichaTecnicaDiagnostico/{clavePlaza}")]
        public ActionResult<Response> InsertFichaTecnicaDiagnostico(string clavePlaza, [FromBody] FichaTecnicaDiagnostico fichaTecnicaDiagnostico)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertDiagnosticoFichaTecnica(clavePlaza, fichaTecnicaDiagnostico);
                if (get.Result == null)
                    return NotFound(get);
                return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("FichaTecnicaDiagnosticoLane/{clavePlaza}")]
        public ActionResult<Response> InsertFichaTecnicaIntervencionLane(string clavePlaza, [FromBody] FichaTecnicaIntervencionLane fichaTecnicaIntervencionLane)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertFichaTecnicaIntervencionLane(clavePlaza, fichaTecnicaIntervencionLane);
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
                string dir = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs";
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

                    FileInfo fi = new FileInfo(Path.Combine(dir, filename));
                    if(fi.Length > 1000000)
                    {
                        string temporal = Path.Combine(dir, filename) + "_temp";
                        using(var imgOrigin = Image.Load(Path.Combine(dir, filename)))
                        {
                            var jpegOptions = new JpegOptions(){
                                CompressionType = Aspose.Imaging.FileFormats.Jpeg.JpegCompressionMode.Progressive
                            };
                            imgOrigin.Save(Path.Combine(dir, temporal), jpegOptions);
                        }
                        if(System.IO.File.Exists(Path.Combine(dir, filename)))
                        {
                            //Se borra archivo grande
                            System.IO.File.Delete(Path.Combine(dir, filename));
                            //Archivo temporal actualiza su nombre al real
                            System.IO.File.Move(Path.Combine(dir, temporal), Path.Combine(dir, filename));
                        }
                    }
                    return Ok(Path.Combine(dir, filename));
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
            }
            else
                return NotFound("Insert another image");
        }

        [AllowAnonymous]
        [HttpGet("Images/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadFichaTecnicaImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs\{fileName}";
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
                string directoy = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs";
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

        [AllowAnonymous]
        [HttpGet("Images/DeleteImg/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<string> DeleteFichaTecnicaImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs").Length == 0)
                    Directory.Delete($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\FichaTecnicaAtencionImgs");
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