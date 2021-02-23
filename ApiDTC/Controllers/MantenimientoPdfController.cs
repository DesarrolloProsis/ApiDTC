namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiDTC.Data;
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
        #endregion
    }
}