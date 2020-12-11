namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;
    
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpPost("{clavePlaza}/{flag}")]
        public ActionResult<Response> Post(string clavePlaza, [FromBody] List<RequestedComponent> requestedComponent, bool flag)
        {
            if(ModelState.IsValid)
            {
                var get = _db.PostRequestedComponent(clavePlaza, requestedComponent, flag);
                if(get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
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
