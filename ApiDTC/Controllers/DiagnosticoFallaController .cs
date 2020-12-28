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
    public class DiagnosticoFallaController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public DiagnosticoFallaController()
        {
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        [HttpGet("{clavePlaza}/{ubicacion}")]
        public IActionResult GetDiagnosticoFalla(string clavePlaza, string ubicacion)
        {
            try
            {
                DiagnosticoFallaPdfCreation pdf = new DiagnosticoFallaPdfCreation(clavePlaza, new ApiLogger(), ubicacion);
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