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

        [HttpPost("nuevo")]
        public ActionResult CreateUser([FromBody] UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                var get = _db.NewUser(userInfo);
                if (get.SqlResult == null)
                    return BadRequest();
                else
                    return StatusCode(201, get);
            }
            return BadRequest(ModelState);
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

        [HttpPut("update")]
        public ActionResult Put([FromBody] UserUpdate userUpdate)
        {
            var put = _db.PutUser(userUpdate);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }

        [HttpPut("newPassword")]
        public ActionResult PutPassword([FromBody] UserPassword userPassword)
        {
            var put = _db.PutPassword(userPassword);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }

        [HttpPut("delete")]
        public ActionResult Delete([FromBody] UserKey userKey)
        {
            if (ModelState.IsValid)
            {
                var delete = _db.DeleteUser(userKey);
                if (delete.SqlResult == null)
                    return NotFound(delete);
                else
                    return Ok(delete);
            }
            return BadRequest(ModelState);
        }
    }
}