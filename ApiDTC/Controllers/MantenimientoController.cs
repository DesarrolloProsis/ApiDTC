namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiDTC.Data;
    using ApiDTC.Services;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MantenimientoController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;

        private readonly MantenimientoDb _db;
        #endregion

        #region Constructor
        public MantenimientoController(MantenimientoDb db)
        {
            this._db = db ?? throw new ArgumentException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        
        #endregion

        #region Methods
        [HttpGet("Bitacora")]
        public ActionResult<Response> GetBitacora()
        {
            var get = _db.GetBitacora();
            if(get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpPost("UpdateFolioFechaInventario/{clavePlaza}/{IdGare}/{NCapufe}/{Fecha}/{Folio}/{IdUsuario}")]
        public ActionResult<Response> UpdateFolioFechaInventario(string clavePlaza, string IdGare, string NCapufe, string Fecha, string Folio, int IdUsuario)
        {
            //string clavePlaza, int IdGare, int NCapufe, string Fecha, string Folio, int IdUsuario
            var put = _db.UpdateFolioFechaInventario(clavePlaza, IdGare, NCapufe, Fecha, Folio, IdUsuario);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);
        }


        #endregion
    }
}