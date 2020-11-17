using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ApiDTC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private IHostingEnvironment _environment;

        public ImageController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        //SquareCatalogId, ReferenceNumber, Imagen
        [HttpPost("InsertImage")]
        public ActionResult<Response> Prueba([FromForm(Name = "image")] IFormFile image, [FromForm(Name = "id")] string referenceNumber, [FromForm(Name = "plaza")] string plaza)
        {
            if (image.Length > 0 || image == null)
            {
                try
                {
                    int numberOfImages;
                    string directoy = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}";

                    if(!Directory.Exists(directoy))
                        Directory.CreateDirectory(directoy);
                    numberOfImages = Directory.GetFiles(directoy).Length + 1;
                    string fileName = $"{referenceNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(directoy, fileName)))
                    {
                        numberOfImages += 1;
                        fileName = $"{referenceNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    var fs = new FileStream(Path.Combine(directoy, fileName), FileMode.Create);
                    //image.CopyTo(new FileStream(Path.Combine(directoy, fileName), FileMode.Create));
                    image.CopyTo(fs);
                    fs.Close();

                    return Ok(directoy);
                }
                catch (IOException ex)
                {
                    return NotFound(ex.ToString());
                }
            }
            else
                return NotFound("Insert another image");
        }

        [HttpGet("DeleteDtcImages/{plaza}/{referenceNumber}/")]
        public ActionResult<DtcImage> Download(string plaza, string referenceNumber)
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
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Download/{plaza}/{referenceNumber}/{fileName}")]
        public ActionResult<DtcImage> Download(string plaza, string referenceNumber, string fileName)
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
                return NotFound(ex.ToString());
            }
        }
        
        [HttpGet("DownloadFile/{plaza}/{referenceNumber}/{fileName}")]
        public IActionResult DownloadFile(string plaza, string referenceNumber, string fileName)
        {
            try
            {
                string route = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\{fileName}";
                if (!System.IO.File.Exists(route))
                    return null;
                Byte[] bitMap = System.IO.File.ReadAllBytes(route);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                return null;
            }
        }

        [HttpGet("DownloadBase/{plaza}/{referenceNumber}/{fileName}")]
        public ActionResult DownloadBase(string plaza, string referenceNumber, string fileName)
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
                return NotFound(ex);
            }
        }

        [HttpGet("GetImages/{plaza}/{referenceNumber}")]
        public ActionResult<List<string>> GetImages(string plaza, string referenceNumber)
        {
            try
            {
                string directoy = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\";
                if (!Directory.Exists(directoy))
                    return NotFound(directoy);
                List<string> dtcImages = new List<string>();
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                return NotFound(ex.ToString());
            }
        }
        //https://localhost:44358/api/image/Tlalpan/TLA-20002/TLA-20002_Image_1.jpg
        [HttpGet("Delete/{plaza}/{referenceNumber}/{fileName}")]
        public ActionResult<string> Delete(string plaza, string referenceNumber, string fileName)
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
                return NotFound(ex.ToString());
            }
        }
    }


}