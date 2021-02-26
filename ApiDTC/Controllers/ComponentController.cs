namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ComponentController : ControllerBase
    {
        #region Attributes
        private readonly ComponentDb _db;
        #endregion
        
        #region Constructor
        public ComponentController(ComponentDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        #region Methods

        // GET: api/Component
        [HttpGet("GetComponetV2/{clavePlaza}/{squareId}/{agreementId}/{attachedId}/{relationShip}/{relationShipPrincipal}")]
        public ActionResult<Response> GetComponents(string clavePlaza, string squareId, int agreementId, int attachedId, int relationShip, int relationShipPrincipal)
        {
            var get = _db.GetComponentDataModificaciones(clavePlaza, squareId, agreementId, attachedId, relationShip, relationShipPrincipal);                            
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
        #endregion
    }
}
