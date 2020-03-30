using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDTC.Data;
using ApiDTC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentController : ControllerBase
    {

        private readonly ComponentDB _db;
        public ComponentController(ComponentDB db)
        {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }

        // GET: api/Component
        [HttpGet("{convenio}/{plaza}/{Id}")]
        public object GetComponents(string convenio, string plaza, 
           string Id)
        {
            return _db.GetComponentData(convenio, plaza, Id);
        }

        //GET: api/Component/5
        [HttpGet]
        public ActionResult<List<SelectListItem>> Get()
        {
            return _db.GetComponentData1();
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
