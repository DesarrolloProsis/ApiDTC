namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using ApiDTC.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        #region Attributes
        private readonly IHostingEnvironment _environment;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public ImageController(IHostingEnvironment environment)
        {
            _environment = environment;
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        [HttpPost("InsertImage/{clavePlaza}")]
        public ActionResult<Response> InsertImage(string clavePlaza, [FromForm(Name = "image")] IFormFile image, [FromForm(Name = "id")] string referenceNumber, [FromForm(Name = "plaza")] string plaza)
        {
            _apiLogger.WriteLog(clavePlaza, "InsertImage", 1, plaza + referenceNumber);
            if (image.Length > 0 || image != null)
            {
                
                int numberOfImages;
                string directoy = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}", fileName;
                _apiLogger.WriteLog(clavePlaza, "InsertImage", 1, directoy);
                try
                {
                    _apiLogger.WriteLog(clavePlaza, "InsertImage", 1, "Evaluacion directorio");
                    if (!Directory.Exists(directoy))
                    {

                        _apiLogger.WriteLog(clavePlaza, "InsertImage", 1, "Se va a crear directorio: " + directoy);
                        Directory.CreateDirectory(directoy);
                        _apiLogger.WriteLog(clavePlaza, "InsertImage", 1, "Se crea directorio: " + directoy);
                    }
                        
                    numberOfImages = Directory.GetFiles(directoy).Length + 1;
                    _apiLogger.WriteLog(clavePlaza, "InsertImage", 1, $"{numberOfImages}");
                    fileName = $"{referenceNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    _apiLogger.WriteLog(clavePlaza, "InsertImage", 1, fileName);
                    while (System.IO.File.Exists(Path.Combine(directoy, fileName)))
                    {
                        numberOfImages += 1;
                        fileName = $"{referenceNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    var fs = new FileStream(Path.Combine(directoy, fileName), FileMode.Create);
                    image.CopyTo(fs);
                    fs.Close();
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "ImageController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
                catch (Exception ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "ImageController: InsertImage", 3);
                    return NotFound(ex.ToString());
                }
                return Ok(directoy);
            }
            else
                return NotFound("Insert another image");   
        }

        [HttpGet("DeleteDtcImages/{clavePlaza}/{plaza}/{referenceNumber}/")]
        public ActionResult<DtcImage> DeleteImage(string clavePlaza, string plaza, string referenceNumber)
        {
            try
            {
                string route = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}";
                if(!System.IO.Directory.Exists(route))
                    return NotFound(route);
                System.IO.Directory.Delete(route, true);
                return Ok(route);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ImageController: DeleteImage", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Download/{clavePlaza}/{plaza}/{referenceNumber}/{fileName}")]
        public ActionResult<DtcImage> Download(string clavePlaza, string plaza, string referenceNumber, string fileName)
        {
            try
            {
                string route = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\{fileName}";
                if (!System.IO.File.Exists(route))
                    return NotFound(route);
                byte[] bitMap = System.IO.File.ReadAllBytes(route);
                var dtcImage = new DtcImage
                {
                    FileName = fileName,
                    Image = Convert.ToBase64String(bitMap)
                };
                return Ok(dtcImage);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ImageController: Download", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("DownloadFile/{clavePlaza}/{plaza}/{referenceNumber}/{fileName}")]
        public IActionResult DownloadFile(string clavePlaza, string plaza, string referenceNumber, string fileName)
        {
            try
            {
                string route = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\{fileName}";
                if (!System.IO.File.Exists(route))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(route);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ImageController: DownloadFile", 2);
                return NotFound(ex.ToString());
            }
        }

        //[HttpGet("DownloadBase/{plaza}/{referenceNumber}/{fileName}")]
        [HttpGet("DownloadBase/{clavePlaza}/{plaza}/{referenceNumber}/{fileName}")]
        public ActionResult DownloadBase(string clavePlaza, string plaza, string referenceNumber, string fileName)
        {
            try
            {
                string route = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\{fileName}";
                if (!System.IO.File.Exists(route))
                    return null;
                Byte[] bitMap = System.IO.File.ReadAllBytes(route);

                return Ok(bitMap);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ImageController: DownloadBase", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("GetImages/{clavePlaza}/{plaza}/{referenceNumber}")]
        public ActionResult<List<string>> GetImages(string clavePlaza, string plaza, string referenceNumber)
        {
            try
            {
                string directoy = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ImageController: GetImages", 2);
                return NotFound(ex.ToString());
            }
        }
        
        [HttpGet("Delete/{clavePlaza}/{plaza}/{referenceNumber}/{fileName}")]
        public ActionResult<string> Delete(string clavePlaza, string plaza, string referenceNumber, string fileName)
        {
            try
            {
                string file = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\{fileName}";
                if (!System.IO.File.Exists(file))
                    return NotFound(file);
                System.IO.File.Delete(file);
                if (System.IO.Directory.GetFiles($@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\").Length == 0)
                    System.IO.Directory.Delete($@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\");
                return Ok(file);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ImageController: Delete", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion
    }
}