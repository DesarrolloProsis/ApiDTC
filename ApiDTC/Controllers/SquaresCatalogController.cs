﻿namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class SquaresCatalogController : ControllerBase
    {
        #region Attributes
        private readonly SquaresCatalogDb _db;
        #endregion

        #region Constructor
        public SquaresCatalogController(SquaresCatalogDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        // GET: api/SquaresCatalog
        //[HttpGet]
        [HttpGet("{clavePlaza}")]
        public ActionResult<Response> Get(string clavePlaza)
        {
            var get = _db.GetSquaresCatalog(clavePlaza);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //[HttpGet("Lanes/{square}")]
        [HttpGet("Lanes/{clavePlaza}/{square}")]
        public ActionResult<Response> GetLanes(string clavePlaza, string square)
        {
            var get = _db.GetLanes(clavePlaza, square);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
    }
}
