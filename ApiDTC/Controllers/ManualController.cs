using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManualController : ControllerBase
    {

        [HttpGet("getManual")]
        public async Task<ActionResult> DescargarManual()
        {
            var filePath = @"D:\BitacoraDesarrollo\Manual_de_usuario_DTC_-_Actualizado.pdf";

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "application/pdf", Path.GetFileName(filePath));
        }

    }
}
