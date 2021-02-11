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

    [Route("api/[controller]")]
    [ApiController]
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
        #endregion
    }
}