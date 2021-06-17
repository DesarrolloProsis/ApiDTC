using ApiDTC.Data;
using ApiDTC.Models;
using ApiDTC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {
        #region Attributes
        private readonly ComentarioDb _db;

        private readonly ApiLogger _apiLogger;

        private readonly string _disk;

        private readonly string _folder;
        #endregion
        public ComentarioController(ComentarioDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        [HttpPost("comment/{clavePlaza}")]
        public ActionResult<Response> insertarComment(string clavePlaza, [FromBody] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertComment(clavePlaza, comment);
                if (get.SqlResult == null)
                    return NotFound(get);
                return StatusCode(200, get);
            }
            return BadRequest(ModelState);
        }

    }
}
