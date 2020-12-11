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

        #region Methods
        //[HttpGet]
        [HttpGet("{clavePlaza}")]
        public ActionResult<Response> Get(string clavePlaza)
        {
            var get = _db.GetTypeDescriptionsData(clavePlaza);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        #endregion
    }
}
