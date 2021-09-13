
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
        [HttpPost]
        public ActionResult<Response> GetLogin([FromBody] LoginUserInfo loginUserInfo)
        {
            var get = _db.GetStoreLogin(loginUserInfo);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        [HttpPost("Refresh")]
        public ActionResult<Response> RefreshToken([FromBody] UserRefreshToken userRefreshToken)
        {
            return _db.RefreshToken(userRefreshToken);
        }
        [HttpPost("Cookie")]
        public ActionResult<Response> Cookie([FromBody] UserRefreshToken userRefreshToken)
        {
            //Forzar con un comentario la actualización de fuente
            var get = _db.GetStoreCookie(userRefreshToken);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        [HttpPost("LoginInfo")]
        public ActionResult<Response> LoginInfo([FromBody] UserRefreshToken userRefreshToken)
        {
            var get = _db.GetStoreLoginInfo(userRefreshToken);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        [HttpGet("buscarTec/{numPlaza}")]
        public ActionResult<Response> BuscarTec(string numPlaza)
        {
            var get = _db.GetTec(numPlaza);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        [HttpGet("buscarHeaderTec/{idTec}")]
        public ActionResult<Response> BuscarHeaderTec(int idTec)
        {
            var get = _db.GetHeadTec(idTec);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        [HttpGet("SesionLog/{userId}/{pagina}/{registros}/{nameFilter?}/{dateFilter?}")]                
        public ActionResult<Response> GetSessionLog(int userId, int pagina, int registros, string nameFilter = null, string dateFilter = null)
        {
            var _nameFlterValid = nameFilter != "null" ? nameFilter : null;
            var _dateFilterValid = dateFilter != "null" ? dateFilter : null;

            var get = _db.GetSesionLog(userId, pagina, registros, _nameFlterValid, _dateFilterValid);
           if (get.Result == null)
                return NotFound(get);
           else
                return Ok(get);
        }

        #endregion
    }
}
