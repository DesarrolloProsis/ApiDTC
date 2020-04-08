namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentController : ControllerBase
    {
        #region Attributes
        private readonly ComponentDb _db;
        #endregion
        
        #region Constructor
        public ComponentController(ComponentDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion
        
        
        // GET: api/Component
        [HttpGet("{convenio}/{plaza}/{Id}")]
        public IActionResult GetComponents(string convenio, string plaza, string Id)
        {
            var response = _db.GetComponentData(convenio, plaza, Id);
            if(response.Result == null)
                return NotFound(response);
            return Ok(response);
        }

        //GET: api/Component/5
        [HttpGet]
        public IActionResult Get()
        {
            var response = _db.GetComponentsData();
            if(response.Result == null)
                return NotFound(response);
            return Ok(response);
        }

        // POST: api/Component
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Component/5
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
