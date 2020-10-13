using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDTC.Data;
using ApiDTC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotDb _db;
        public BotController(BotDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet("UpdateStatus/{Key}/{IdUser}")]
        public RedirectResult UpdateStatus(string Key, int IdUser)
        {
            bool get = _db.UpdateUserStatus(Key, IdUser);
            if (!get)
                return Redirect("https://www.facebook.com");//404
            else
                return Redirect("https://www.google.com");//200
        }

        [HttpGet("DeleteUser/{Key}/{IdUser}")]
        public RedirectResult DeleteUser(string Key, int IdUser)
        {
            bool get = _db.DeleteUser(Key, IdUser);
            if (!get)
                return Redirect("https://www.facebook.com");
            else
                return Redirect("https://www.google.com");
        }
    }
}