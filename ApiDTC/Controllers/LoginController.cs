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
    public class LoginController : ControllerBase
    {
        #region Attributes
        private readonly LoginDb _db;
        #endregion
        
        #region Constructor
        public LoginController(LoginDb db) 
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        #region Methods
        [HttpPost]
        public ActionResult<Response> GetLogin([FromBody] LoginUserInfo loginUserInfo)
        {        
            var get = _db.GetStoreLogin(loginUserInfo);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        // GET: api/Login
        [HttpPost("ValidUser")]
        public ActionResult<Response> ValidUser([FromBody] LoginUserInfo loginUserInfo)
        {
            var get = _db.GetStoreLoginCookie(loginUserInfo);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        // GET: api/Login
        //Regresa técnicos de plaza
        [HttpGet("buscarTec/{numPlaza}")]
        public ActionResult<Response> BuscarTec(string numPlaza)
        {
            var get = _db.GetTec(numPlaza);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        // GET: api/Login
        [HttpGet("buscarHeaderTec/{idTec}")]
        public ActionResult<Response> BuscarHeaderTec(int idTec)
        {
            var get = _db.GetHeadTec(idTec);
            if(get.Result == null)
                return NotFound(get);
            else   
                return Ok(get);
        }
        #endregion
    }
}
