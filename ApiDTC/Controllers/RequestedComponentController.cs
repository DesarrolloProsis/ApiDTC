namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestedComponentController : ControllerBase
    {
        #region Attributes
        private readonly RequestedComponentDb _db;
        #endregion
        
        #region Constructor
        public RequestedComponentController(RequestedComponentDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        Log.Logs log = new Log.Logs();
        int contadorPartidas = 0, contadorComponentes = 0;

        //[HttpPost("{flag}")]
        [HttpPost("{clavePlaza}/{flag}/{numPartidas}/{numComponentes}")]
        public ActionResult<Response> Post(string clavePlaza, [FromBody] List<RequestedComponent> requestedComponent, bool flag, int numPartidas, int numComponentes)
        {
            try
            {
                foreach (var item in requestedComponent)
                {
                    if (item.IntPartida != 0)
                    {
                        contadorPartidas = contadorPartidas + item.IntPartida;
                    }
                    else if (item.ComponentsStockId != 0)
                    {
                        contadorComponentes = contadorComponentes + item.ComponentsStockId;
                    }
                }

                if (contadorPartidas == numPartidas)
                {
                    if (contadorComponentes == numComponentes)
                    {
                        if (ModelState.IsValid)
                        {
                            var get = _db.PostRequestedComponent(clavePlaza, requestedComponent, flag);
                            if (get.Result == null)
                                return BadRequest(get);
                            else
                                return Ok(get);
                        }
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        log.CreateFileNoError(this, "El numero de componentes no coinciden con el numero de componentes enviados, Numeros de componentes: " + contadorComponentes + " Numero de componentes enviados: " + numComponentes);
                        return BadRequest();
                    }
                }
                else
                {
                    log.CreateFileNoError(this, "El numero de partidas no coinciden con el numero de partidas enviadas, Partidas enviadas: " + contadorPartidas + " Numero de partidas enviadas: " + numPartidas);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                log.CreateFile(this, ex);
                return BadRequest();
            }
        }

        //[HttpPost("Open/{flag}")]
        [HttpPost("Open/{clavePlaza}/{flag}")]
        public ActionResult<Response> PostOpen(string clavePlaza, [FromBody] List<RequestedComponentOpen> requestedComponent, bool flag)
        {
            if (ModelState.IsValid)
            {
                var get = _db.PostRequestedComponentOpen(clavePlaza, requestedComponent, flag);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }
    }
}
