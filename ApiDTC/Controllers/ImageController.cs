using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ApiDTC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Drawing;

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

        [HttpPost("Prueba")]
        public ActionResult<string> Prueba([FromForm] TestImage file)
        {
            if (file.Image.Length > 0 || file == null)
            {
                try
                {
                    var fileName = Path.GetFileName(file.Image.FileName);
                    string contentType = file.Image.ContentType;
                    if (!Directory.Exists(_environment.WebRootPath + "DtcImages"))
                        Directory.CreateDirectory(_environment.WebRootPath + "DtcImages");

                    file.Image.CopyTo(new FileStream(Path.Combine(_environment.WebRootPath + "DtcImages", fileName), FileMode.Create));
                    return "bien";
                }
                catch (IOException ex)
                {
                    return ex.ToString();
                }
            }
            else
                return "WTF";
        }

        [HttpGet("Download")]
        public IActionResult Download()
        {
            Byte[] bitMap;
            bitMap = System.IO.File.ReadAllBytes(_environment.ContentRootPath + "\\DtcImages\\ero.jpeg");
            return File(bitMap, "image/jpeg");
        }
    }


}