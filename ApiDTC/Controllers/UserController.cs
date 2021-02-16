﻿namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Attributes
        private readonly UserDb _db;
        #endregion

        #region Constructor
        public UserController(UserDb db, IConfiguration configuration)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        [HttpPost("nuevo")]
        public ActionResult<Response> CreateUser([FromBody] UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                var get = _db.NewUser(userInfo);
                if (get.Result == null)
                    return BadRequest();
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("consulta")]
        public ActionResult<Response> GetUserInfo([FromBody] UserKey userKey)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetInfo(userKey);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("update")]
        public ActionResult<Response> Put([FromBody] UserUpdate userUpdate)
        {
            var put = _db.PutUser(userUpdate);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }

        [HttpPut("newPassword")]
        public ActionResult<Response> PutPassword([FromBody] UserPassword userPassword)
        {
            var put = _db.PutPassword(userPassword);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }

        [HttpPut("delete")]
        public ActionResult<Response> Delete([FromBody] UserKey userKey)
        {
            if (ModelState.IsValid)
            {
                var delete = _db.DeleteUser(userKey);
                if (delete.Result == null)
                    return NotFound(delete);
                else
                    return Ok(delete);
            }
            return BadRequest(ModelState);
        }
    }
}