namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestedComponentController : ControllerBase
    {
        #region Attributes
        private readonly RequestedComponentDb _db;
        #endregion
        
        #region Constructor
        public RequestedComponentController(RequestedComponentDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        //[HttpPost("{flag}")]
        [HttpPost("{clavePlaza}/{flag}/{elementosEnviados}")]
        public ActionResult<Response> Post(string clavePlaza, [FromBody] List<RequestedComponent> requestedComponent, bool flag, int elementosEnviados)
        {
            try
            {
                if (requestedComponent.Count() == elementosEnviados)
                {
                    if (ModelState.IsValid)
                    {
                        var get = _db.PostRequestedComponent(clavePlaza, requestedComponent, flag);
                        if (get.Result == null)
                            return BadRequest(get);
                        else
                            return Ok(get);
                    }
                    return BadRequest(ModelState);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Log.Logs log = new Log.Logs();
                log.CreateFile(this, ex);
                return BadRequest();
            }
        }

        //[HttpPost("Open/{flag}")]
        [HttpPost("Open/{clavePlaza}/{flag}")]
        public ActionResult<Response> PostOpen(string clavePlaza, [FromBody] List<RequestedComponentOpen> requestedComponent, bool flag)
        {
            if (ModelState.IsValid)
            {
                var get = _db.PostRequestedComponentOpen(clavePlaza, requestedComponent, flag);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
    }
}
