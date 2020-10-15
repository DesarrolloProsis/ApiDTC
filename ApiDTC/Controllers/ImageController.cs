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
        public ActionResult<string> Prueba([FromForm(Name = "file")] IFormFile file, [FromForm(Name = "id")] string reference_number, [FromForm(Name = "plaza")] string plaza, [FromForm(Name = "name")] string name)
        {
            //Sin suerte en enviar multiples imagenes :(
            if (file.Length > 0 || file == null)
            {
                try
                {
                    var fileName = name;            
                    string contentType = file.ContentType;

                    if (!Directory.Exists(_environment.WebRootPath + "DtcImages\\" + plaza + "\\" + reference_number))
                        Directory.CreateDirectory(_environment.WebRootPath + "DtcImages\\" + plaza + "\\" + reference_number);

                    //var myFile = new FileStream(Path.Combine(_environment.WebRootPath + "DtcImages\\" + plaza + "\\" + reference_number, fileName), FileMode.Create);
                    file.CopyTo(new FileStream(Path.Combine(_environment.WebRootPath + "DtcImages\\" + plaza + "\\" + reference_number, fileName), FileMode.Create));
                        
                    //myFile.CopyTo(new FileStream(Path.Combine(_environment.WebRootPath + "DtcImages\\" + plaza + "\\" + reference_number, fileName), FileMode.Create));

                    
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
            bitMap = System.IO.File.ReadAllBytes(_environment.ContentRootPath + "\\DtcImages\\Tlalpan\\TLA-20002\\img1.jpg");
            return File(bitMap, "image/jpeg");
        }
    }


}