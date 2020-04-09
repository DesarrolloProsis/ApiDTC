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

        private readonly LoginDb _db;
        public LoginController(LoginDb db) {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }

        // GET: api/Login
        [HttpGet("{userName}/{passWord}/{flag}")]
        public IActionResult GetLogin(string userName, string passWord, bool flag)
        {        
            var response = _db.GetStoreLogin(userName, passWord, flag);
            if(response.Result == null)
                return NotFound(response);
            else
                return Ok(response);
        }
        // GET: api/Login
        [HttpGet("ValidUser/{userName}/{passWord}/{flag}")]
        public ActionResult<SqlResult> GetCookie(string userName, string passWord, bool flag)
        {
            return _db.GetStoreLoginCookie(userName, passWord, flag);

        }
        // GET: api/Login
        //Regresa técnicos de plaza
        [HttpGet("buscarTec/{numPlaza}")]
        public ActionResult<SqlResult> GetCookie(string numPlaza)
        {

            return _db.GetTec(numPlaza);
                

        }
        // GET: api/Login
        [HttpGet("buscarHeaderTec/{idTec}")]
        public ActionResult<SqlResult> GetCokie(int idTec)
        {
            return _db.GetHeadTec(idTec);
        }
       
    }
}
