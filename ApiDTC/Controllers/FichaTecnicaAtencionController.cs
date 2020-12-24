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
        #endregion
    }
}