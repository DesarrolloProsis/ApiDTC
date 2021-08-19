using System;
using System.Collections.Generic;
using ApiDTC.Data;
using ApiDTC.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ComponentController : ControllerBase
    {
        #region Attributes
        private readonly ComponentDb _db;
        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor
        public ComponentController(ComponentDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }
        #endregion

        #region Methods

        // GET: api/Component
        [HttpGet("GetComponetV2/{clavePlaza}/{squareId}/{agreementId}/{attachedId}/{relationShip}/{relationShipPrincipal}/{componentsStockId}")]
        public ActionResult<Response> GetComponents(string clavePlaza, string squareId, int agreementId, int attachedId, int relationShip, int relationShipPrincipal, int componentsStockId)
        {
            var get = _db.GetComponentDataModificaciones(clavePlaza, squareId, agreementId, attachedId, relationShip, relationShipPrincipal, componentsStockId);                            
            return Ok(get);
        }

        // GET: api/Component
        //[HttpGet("{convenio}/{plaza}/{Id}/{marca}")]
        [HttpGet("{clavePlaza}/{convenio}/{plaza}/{Id}/{marca}")]
        public ActionResult<Response> GetComponents(string clavePlaza, string convenio, string plaza, string Id, string marca)
        {
            var get = _db.GetComponentData(clavePlaza, convenio, plaza, Id, marca);
            if(get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        //GET: api/Component/5
        //[HttpGet("{idAgreement}")]
        [HttpGet("{clavePlaza}/{idAgreement}")]
        public ActionResult<Response> Get(string clavePlaza, int idAgreement)
        {
            var get = _db.GetComponentsData(clavePlaza, idAgreement);
            if(get.Result == null)
                return NotFound(get);
            return Ok(get);
        }
        //GET: api/Component/5
        //[HttpGet("versionProduccion/{plaza}/{convenio}")]
        [HttpGet("versionProduccion/{clavePlaza}/{plaza}/{convenio}")]
        public ActionResult<Response> Get(string clavePlaza, string plaza, string convenio)
        {
            var get = _db.VersionPruebaComponet(clavePlaza, plaza, convenio);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        //[HttpGet("Inventario/{plaza}")]
        [HttpGet("Inventario/{clavePlaza}/{plaza}")]
        public ActionResult<Response> GetComponentsInventory(string clavePlaza, string plaza)
        {
            var get = _db.GetComponentsInventory(clavePlaza, plaza);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        //[HttpGet("InventarioUbicacion")]
        [HttpGet("InventarioUbicacion/{clavePlaza}")]
        public ActionResult<Response> GetComponentsInventoryUbication(string clavePlaza)
        {
            var get = _db.GetComponentsInventoryUbication(clavePlaza);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        //[HttpGet("Inventario/{componente}/{plaza}")]
        [HttpGet("Inventario/{clavePlaza}/{componente}/{plaza}")]
        public ActionResult<Response> GetComponentsInventoryLane(string clavePlaza, string componente, string plaza)
        {
            var get = _db.GetComponentsInventoryLane(clavePlaza, componente, plaza);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        //[HttpGet("Inventario/{componente}/{linea}/{squareId}")]
        [HttpGet("Inventario/{clavePlaza}/{componente}/{linea}/{squareId}")]
        public ActionResult<Response> GetComponentsInventoryDescription(string clavePlaza, string componente, string linea, string squareId)
        { 
            var get = _db.GetComponentsInventoryDescription(clavePlaza, componente, linea, squareId);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        // PUT: api/Component/5
        //[HttpPut("updateInventory")]
        [HttpPut("updateInventory/{clavePlaza}")]
        public ActionResult<Response> Put(string clavePlaza, [FromBody] UpdateInventory updateInventory)
        {
            var put = _db.PutComponentInventary(clavePlaza, updateInventory);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }


        //[HttpPut("updateInventoryList")]
        [HttpPut("updateInventoryList/{clavePlaza}")]
        public ActionResult<Response> Put(string clavePlaza, [FromBody] List<ComponentsInventoryList> componentsInventoryList)
        {
            var put = _db.InventoryListUpdate(clavePlaza, componentsInventoryList);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }

        [HttpPost("updateInventory/{clavePlaza}/{Reference}/{UserId}")]
        public ActionResult<Response> UpdateInventory(string clavePlaza, string Reference, int UserId)
        {
            var put = _db.UpdateInventory(clavePlaza, Reference, UserId);
            if (put.SqlResult == null)
                return NotFound(put);
            return Ok(put);

        }
        [HttpGet("ReporteComponente/")]
        public ActionResult<Response> GetReporteComponent()
        {
            var get = _db.GetReporteComponente("ReporteComponentes");
            if (get.Result == null)
                return NotFound(get);
            Console.WriteLine(this._disk);
            //this.GetReporteComponentExcel();
            
            return Ok(get);
        }

        [HttpGet("DescargarExcel/")]
        public IActionResult GetReporteComponentExcel()
        {
            var get = _db.GetReporteComponente("ReporteComponentes");
            if (get.Result == null)
            {
                return null;
            }
            else
            {
                List<ReporteComponente> lista = (List<ReporteComponente>)get.Result;
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        int fila = 2;
                        var ws = workbook.Worksheets.Add("Lista");
                        ws.Cell(1, 1).Value = "Plaza";
                        ws.Cell(1, 2).Value = "Carril";
                        ws.Cell(1, 3).Value = "Componente";
                        ws.Cell(1, 4).Value = "Cantidad";
                        ws.Cell(1, 5).Value = "Precio";
                        ws.Cell(1, 6).Value = "Solicitante";
                        ws.Cell(1, 7).Value = "TipoDTC";
                        ws.Cell(1, 8).Value = "Referencia";
                        foreach (ReporteComponente item in lista)
                        {
                            ws.Cell(fila, 1).Value = item.Plaza;
                            ws.Cell(fila, 2).Value = item.Carril;
                            ws.Cell(fila, 3).Value = item.Componente;
                            ws.Cell(fila, 4).Value = item.Cantidad;
                            ws.Cell(fila, 5).Value = item.Precio;
                            ws.Cell(fila, 6).Value = item.Solicitante;
                            ws.Cell(fila, 7).Value = item.TipoDTC;
                            ws.Cell(fila, 8).Value = item.Referencia;
                            fila++;

                        }
                        workbook.SaveAs("D:\\HelloWorld.xlsx");


                        //return File(new FileStream("D:\\HelloWorld.xlsx", FileMode.Open, FileAccess.Read), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        byte[] bytes = System.IO.File.ReadAllBytes("D:\\HelloWorld.xlsx");
                        return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ReporteComponentes.xlsx");
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }


        }
        [HttpGet("ComponentStock/{DelegationId}")]
        public ActionResult<Response> GetListComponentStock(int DelegationId)
        {
            var get = _db.GetListComponentStock(DelegationId);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpPost("ComponentAdd/{clavePlaza}/{nInserciones}")]
        public ActionResult<Response> ComponentAdd(string clavePlaza, int nInserciones, [FromBody] ComponentIns Componente)
        {
            var put = _db.DuplicarComponentAdd(clavePlaza, Componente, nInserciones);
            if (put.SqlResult == null)
                return NotFound(put);
            return Ok(put);

        }
        #endregion
    }
}
