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
        [HttpPost]
        public ActionResult Post([FromBody] List<RequestedComponent> requestedComponent)
        {
            if(ModelState.IsValid)
            {
                var get = _db.PostRequestedComponent(requestedComponent);
                if(get.SqlResult == null)
                    return BadRequest(get);
                else
                    return StatusCode(201, get);
            }
            return BadRequest(ModelState);
            //return new object { };
        }

        // PUT: api/RequestedComponent/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
