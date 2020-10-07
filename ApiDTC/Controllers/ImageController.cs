using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiDTC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public static IHostingEnvironment _environment;

        public ImageController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("Prueba")]
        public async Task<string> Prueba([Bind("files")] TestImage files)
        {
            if (files.files.Length > 0)
            {
                try
                {
                    if (!Directory.Exists($"{_environment.WebRootPath}\\imagenDemo\\"))
                        Directory.CreateDirectory($"{_environment.WebRootPath}\\imagenDemo\\");
                    using (FileStream fileStream = System.IO.File.Create($"{_environment.WebRootPath}\\imagenDemo\\{files.files.FileName}"))
                    {
                        await files.files.CopyToAsync(fileStream);
                        fileStream.Flush();
                        return $"\\imagenDemo\\{files.files.FileName}";
                    }
                }
                catch (IOException ex)
                {
                    return ex.ToString();
                }
            }
            else
                return "WTF";
        }
    }
}