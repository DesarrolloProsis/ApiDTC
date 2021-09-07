namespace ApiDTC.Controllers
{
    using System;
    using System.Threading.Tasks;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using ApiDTC.Utilities;
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
                var delete = _db.RevokeUser(userKey, false);
                if (delete.Result == null)
                    return NotFound(delete);
                else
                    return Ok(delete);
            }
            return BadRequest(ModelState);
        }
        [HttpPut("active")]
        public ActionResult<Response> ActivarUser([FromBody] UserKey userKey)
        {
            if (ModelState.IsValid)
            {
                var delete = _db.RevokeUser(userKey, true);
                if (delete.Result == null)
                    return NotFound(delete);
                else
                    return Ok(delete);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("getUserOfPlaza")]

        public GenericResponse<string> GetUsersOfSquare(int IdSquare)
        {
            
            GenericResponse<string> respuesta = new GenericResponse<string>();
            respuesta.Code = "ok";
            respuesta.Result = "Aqui van los valores de la respuesta";
            respuesta.Message = "todo cool";

            return respuesta;
        }
        //Agregar plaza
        [HttpPost("AddSquareToUser")]
        public ActionResult<Response> AddSquareToUser([FromBody] UserSquare userInfo)
        {
            if (ModelState.IsValid)
            {
                var get = _db.AddSquareToUser(userInfo);
                if (get.Result == null)
                    return BadRequest();
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
        //Activar Usuario
        [HttpPut("ActivateUser/{clavePlaza}/{UserId}")]
        public ActionResult<Response> ActivateUser(string clavePlaza, int UserId)
        {
            if (ModelState.IsValid)
            {
                var get = _db.ActivateUser(clavePlaza, UserId);
                if (get.Result == null)
                    return BadRequest();
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        
        [HttpGet("UserOfSquare/{SquareId}")]
        public ActionResult<Response> GetUserOfSquare(string SquareId)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetUserOfSquare(SquareId);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

    }
}