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
        [HttpPost("Prueba")]
        public ActionResult Prueba([FromForm(Name = "image")] IFormFile image, [FromForm(Name = "id")] string referenceNumber, [FromForm(Name = "plaza")] string plaza)
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

                    image.CopyTo(new FileStream(Path.Combine(directoy, fileName), FileMode.Create));

                    return Ok();
                }
                catch (IOException ex)
                {
                    return NotFound(ex.ToString());
                }
            }
            else
                return NotFound("Insert another image");
        }

        [HttpGet("Download/{plaza}/{referenceNumber}")]
        public ActionResult<List<string>> Download(string plaza, string referenceNumber)
        {
            try
            {
                string directoy = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}";
                List<string> files = new List<string>();
                foreach (var item in Directory.GetFiles(directoy))
                {
                    Byte[] bitMap;
                    bitMap = System.IO.File.ReadAllBytes(item);
                    files.Add(Convert.ToBase64String(bitMap));
                }
                return Ok(files);
                //return File(bitMap, "image/jpeg");
            }
                catch (IOException ex)
            {
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("DeleteImage/{plaza}/{referenceNumber}/{fileName}")]
        public ActionResult Delete(string plaza, string referenceNumber, string fileName)
        {
            try
            {
                string directoy = $@"{_environment.WebRootPath}DtcImages\{plaza}\{referenceNumber}\{fileName}";
                List<string> files = new List<string>();
                foreach (var item in Directory.GetFiles(directoy))
                {
                    Byte[] bitMap;
                    bitMap = System.IO.File.ReadAllBytes(item);
                    files.Add(Convert.ToBase64String(bitMap));
                }
                return Ok(files);
                //return File(bitMap, "image/jpeg");
            }
            catch (IOException ex)
            {
                return NotFound(ex.ToString());
            }
        }
    }


}