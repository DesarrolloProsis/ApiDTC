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

        // GET: api/Login
        [HttpGet("{userName}/{passWord}/{flag}")]
        public ActionResult<Response> GetLogin(string userName, string passWord, bool flag)
        {        
            var get = _db.GetStoreLogin(userName, passWord, flag);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        // GET: api/Login
        [HttpGet("ValidUser/{userName}/{passWord}/{flag}")]
        public ActionResult<Response> GetCookie(string userName, string passWord, bool flag)
        {
            var get = _db.GetStoreLoginCookie(userName, passWord, flag);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        // GET: api/Login
        //Regresa técnicos de plaza
        [HttpGet("buscarTec/{numPlaza}")]
        public ActionResult<Response> GetCookie(string numPlaza)
        {
            var get = _db.GetTec(numPlaza);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        // GET: api/Login
        [HttpGet("buscarHeaderTec/{idTec}")]
        public ActionResult<Response> GetCookie(int idTec)
        {
            var get = _db.GetHeadTec(idTec);
            if(get.Result == null)
                return NotFound(get);
            else   
                return Ok(get);
        }
       
    }
}
