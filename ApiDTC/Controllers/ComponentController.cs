namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;
    
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
            var get = _db.GetComponentData(convenio, plaza, Id);
            if(get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        //GET: api/Component/5
        [HttpGet("{plaza}/{numConvenio}")]
        public ActionResult<Response> Get(int plaza, string numConvenio)
        {
            var get = _db.GetComponentsData(plaza, numConvenio);
            if(get.Result == null)
                return NotFound(get);
            return Ok(get);
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
