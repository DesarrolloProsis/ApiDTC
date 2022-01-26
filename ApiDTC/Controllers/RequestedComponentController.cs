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
        string referencia;
        int i = 1;

        string listaDeComponentes = "", componente = "";
        Log.Logs logs = new Log.Logs();
        int j = 1;

        //[HttpPost("{flag}")]
        [HttpPost("{clavePlaza}/{flag}/{numPartidas}/{numComponentes}")]
        public ActionResult<Response> Post(string clavePlaza, [FromBody] List<RequestedComponent> requestedComponent, bool flag, int numPartidas, int numComponentes)
        {
            try
            {
                foreach (var item in requestedComponent)
                {
                    componente = "ComponentsStockId: " + Convert.ToString(item.ComponentsStockId) + "\n" + "\t\t" +
                                 "ReferenceNumber: " + Convert.ToString(item.ReferenceNumber) + "\n" + "\t\t" +
                                 "CapufeLaneNum: " + Convert.ToString(item.CapufeLaneNum) + "\n" + "\t\t" +
                                 "IdGare: " + Convert.ToString(item.IdGare) + "\n" + "\t\t" +
                                 "Marca: " + Convert.ToString(item.Marca) + "\n" + "\t\t" +
                                 "Modelo: " + Convert.ToString(item.Modelo) + "\n" + "\t\t" +
                                 "MarcaPropuesta: " + Convert.ToString(item.MarcaPropuesta) + "\n" + "\t\t" +
                                 "ModeloPropuesto: " + Convert.ToString(item.ModeloPropuesto) + "\n" + "\t\t" +
                                 "NumSerie: " + Convert.ToString(item.NumSerie) + "\n" + "\t\t" +
                                 "Unity: " + Convert.ToString(item.Unity) + "\n" + "\t\t" +
                                 "DateInstallationDate: " + Convert.ToString(item.DateInstallationDate) + "\n" + "\t\t" +
                                 "DateMaintenanceDate: " + Convert.ToString(item.DateMaintenanceDate) + "\n" + "\t\t" +
                                 "MaintenanceFolio: " + Convert.ToString(item.MaintenanceFolio) + "\n" + "\t\t" +
                                 "IntLifeTimeExpected: " + Convert.ToString(item.IntLifeTimeExpected) + "\n" + "\t\t" +
                                 "strLifeTimeReal: " + Convert.ToString(item.strLifeTimeReal) + "\n" + "\t\t" +
                                 "IntPartida: " + Convert.ToString(item.IntPartida) + "\n" + "\t\t" +
                                 "MainRelationship: " + Convert.ToString(item.MainRelationship) + "\n" + "\t\t" +
                                 "TableFolio: " + Convert.ToString(item.TableFolio) + "\n" + "\t\t" +
                                 "Amount: " + Convert.ToString(item.Amount) + "\n" + "\t\t" +
                                 "Unity: " + Convert.ToString(item.Unity);

                    listaDeComponentes = $"{listaDeComponentes} \n\n Partida: {item.IntPartida} \n Componente: {j} \n\t\t{componente}";
                    j++;

                    referencia = item.ReferenceNumber;

                    if (item.IntPartida == i)
                    {
                        contadorPartidas = item.IntPartida;
                        i++;
                    }

                    if (requestedComponent.Count() != 0)
                    {
                        contadorComponentes++;
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
                        log.CreateFileReference(this, "El numero de componentes no coinciden con el numero de componentes enviados, Numeros de componentes insertados: " + contadorComponentes + ", Numero de componentes enviados: " + numComponentes + ", Numero de referencia: " + referencia + listaDeComponentes, referencia);
                        return BadRequest();
                    }
                }
                else
                {
                    log.CreateFileReference(this, "El numero de partidas no coinciden con el numero de partidas enviadas, Numero de partidas insertados: " + contadorPartidas + ", Numero de partidas enviadas: " + numPartidas + ", Numero de referencia: " + referencia + listaDeComponentes, referencia);
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
