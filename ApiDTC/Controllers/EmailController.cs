using ApiDTC.Data;
using ApiDTC.Models.Email;
using ApiDTC.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmailController : Controller
    {
        #region Attributes
        private readonly EmailDb _emailDb;
        #endregion

        #region Constructor
        public EmailController(EmailDb emailDb)
        {
            this._emailDb = emailDb ?? throw new ArgumentNullException(nameof(emailDb));
        }
        #endregion

        #region Methods
        [HttpPost]
        public IActionResult Index([FromBody] Email emailB)
        {
            EmailService email = new EmailService(new ApiLogger());
            if (email.Send(emailB))
                return Ok();
            return NotFound(); ;
        }

        //[HttpGet("Axel")]
        //public IActionResult IndexAxel()
        //{
        //    EmailService email = new EmailService(new ApiLogger());
        //    if (email.Send("axel.frias257@gmail.com", "HOLA RODRIGO ;)", "rodrigo"))
        //        return Ok();
        //    return NotFound(); ;
        //}
        #endregion
    }
}
