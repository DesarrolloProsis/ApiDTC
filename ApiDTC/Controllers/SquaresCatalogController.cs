namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public ActionResult<Response> Get(string clavePlaza)
        {
            var get = _db.GetSquaresCatalog(clavePlaza);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("Lanes/{square}")]
        public ActionResult<Response> GetLanes(string square)
        {
            var get = _db.GetLanes(square);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("Admins/{userId}")]
        public ActionResult<Response> GetLanes(int userId)
        {
            var get = _db.GetAdmins(userId);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpPost("InsertAdmin")]
        public ActionResult<Response> InsertAdmin([FromBody] InsertAdmin insertAdmin)
        {
            if(ModelState.IsValid)
            {
                var post = _db.InsertAdmin(insertAdmin);
                if(post.Result == null)
                    return NotFound(post);
                return Ok(post);
            }
            return BadRequest();
        }

        [HttpPut("UpdateAdmin")]
        public ActionResult<Response> UpdateAdmin([FromBody] UpdateAdmin updateAdmin)
        {
            if(ModelState.IsValid)
            {
                var put = _db.UpdateAdmin(updateAdmin);
                if(put.Result == null)
                    return NotFound(put);
                return Ok(put);
            }
            return BadRequest();
        }

        [HttpPut("UpdateAdminStatus")]
        public ActionResult<Response> UpdateAdminStatus([FromBody] UpdateAdminStatus updateAdminStatus)
        {
            if(ModelState.IsValid)
            {
                var put = _db.UpdateAdminStatus(updateAdminStatus);
                if(put.Result == null)
                    return NotFound(put);
                return Ok(put);
            }
            return BadRequest();
        }
    }
}
