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

        //TODO POST RequestedComponents
        // POST: api/RequestedComponent
        [HttpPost("{flag}")]
        public ActionResult Post([FromBody] List<RequestedComponent> requestedComponent, bool flag)
        {
            if(ModelState.IsValid)
            {
                var get = _db.PostRequestedComponent(requestedComponent, flag);
                if(get.SqlResult == null)
                    return BadRequest(get);
                else
                    return StatusCode(201, get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Open/{flag}")]
        public ActionResult PostOpen([FromBody] List<RequestedComponentOpen> requestedComponent, bool flag)
        {
            if (ModelState.IsValid)
            {
                var get = _db.PostRequestedComponentOpen(requestedComponent, flag);
                if (get.SqlResult == null)
                    return BadRequest(get);
                else
                    return StatusCode(201, get);
            }
            return BadRequest(ModelState);
        }
    }
}
