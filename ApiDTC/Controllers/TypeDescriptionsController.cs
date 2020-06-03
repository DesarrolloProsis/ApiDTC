namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class TypeDescriptionsController : ControllerBase
    {
        #region Attributes
        private readonly TypeDescriptionsDb _db;
        #endregion

        #region  Constructor
        public TypeDescriptionsController(TypeDescriptionsDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        // GET: api/TypeDescriptions
        [HttpGet]
        public ActionResult<Response> Get()
        {
            var get = _db.GetTypeDescriptionsData();
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        // GET: api/TypeDescriptions/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

  
    }
}
