namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDb _db;

        public UserController(UserDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpPost("consulta")]
        public ActionResult GetUserInfo([FromBody] UserKey userKey)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetInfo(userKey);
                if (get.Result == null)
                    return BadRequest();
                else
                    return StatusCode(201, get);
            }
            return BadRequest(ModelState);
        }
    }
}