namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class SquaresCatalogController : ControllerBase
    {
        #region Attributes
        private readonly SquaresCatalogDb _db;
        #endregion

        #region Constructor
        public SquaresCatalogController(SquaresCatalogDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        // GET: api/SquaresCatalog
        [HttpGet]
        public ActionResult<Response> Get()
        {
            var get = _db.GetSquaresCatalog();
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        // POST: api/SquaresCatalog
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/SquaresCatalog/5
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
