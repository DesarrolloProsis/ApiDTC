namespace ApiDTC.Controllers
{
    using System;
    using System.IO;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MantenimientoPdfController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;

        private readonly MantenimientoPdfDb _db;

        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor
        public MantenimientoPdfController(MantenimientoPdfDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods
        [HttpGet("{clavePlaza}/{tipo}/{noReporte}")]
        public IActionResult GetMantenimiento(int tipo, string clavePlaza, string noReporte)
        {
            try
            {
                var get = _db.GetStorePDF(clavePlaza, noReporte);
                if (get.Tables[0].Rows.Count == 0 || get.Tables[1].Rows.Count == 0)
                    return NotFound();
                MantenimientoPdfCreation pdf = new MantenimientoPdfCreation(clavePlaza, get.Tables[0], get.Tables[1], new ApiLogger(), tipo, noReporte);
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "MantenimientoPdf: GetMantenimiento", 2);
                return NotFound(ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpPost("CalendarioEscaneado/{clavePlaza}/{_noReporte}")]
        public ActionResult<Response> SubirTablaActividadesMantenimiento([FromForm(Name = "file")] IFormFile file, string clavePlaza, string _noReporte)
        {

            if (file.Length > 0 || file != null)
            {
                if (file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{_noReporte}\";
                    string filename = $"{_noReporte}-Escaneado.pdf";
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        var fs = new FileStream(Path.Combine(path, filename), FileMode.Create);
                        file.CopyTo(fs);
                        fs.Close();
                        return Ok(path);
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "Mantenimiento: SubirTablaActividadesMantenimiento", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }
        #endregion
    }
}