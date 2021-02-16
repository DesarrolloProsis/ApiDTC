
namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        #region Attributes
        private readonly LoginDb _db;
        
        private string _hash;
        #endregion
        
        #region Constructor
        public LoginController(LoginDb db, IConfiguration configuration) 
        {
            this._hash = Convert.ToString(configuration.GetValue<string>("JWT:key"));
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        #region Methods

        [HttpGet]
        public string Prueba(){
            return this._hash;
        }
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
        public ActionResult<Response> ValidUser(string userName, string passWord, bool flag)
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
