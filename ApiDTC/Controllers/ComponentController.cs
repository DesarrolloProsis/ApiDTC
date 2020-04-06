namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using ApiDTC.Data;
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
        public object GetComponents(string convenio, string plaza, string Id)
        {
            return _db.GetComponentData(convenio, plaza, Id);
        }

        //GET: api/Component/5
        [HttpGet]
        public ActionResult<List<SelectListItem>> Get()
        {
            return _db.GetComponentsData();
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
