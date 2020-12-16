using ApiDTC.Data;
using ApiDTC.Services;
using MailKit.Net.Smtp;
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
        [HttpGet]
        public IActionResult Index()
        {
            EmailService email = new EmailService(new ApiLogger());
            if (email.Send("a.mitra2311@gmail.com", "HOLA CARA DE BOLA", "EMI"))
                return Ok();
            return NotFound();;
        }
        #endregion
    }
}
