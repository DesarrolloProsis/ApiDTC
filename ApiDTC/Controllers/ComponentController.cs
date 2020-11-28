namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;
    
    [Route("api/[controller]")]
    [ApiController]
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
        

        // GET: api/Component
        [HttpGet("GetComponetV2/{squareId}/{agreementId}/{attachedId}/{relationShip}/{relationShipPrincipal}")]
        public ActionResult<Response> GetComponents(string squareId, int agreementId, int attachedId, int relationShip, int relationShipPrincipal)
        {
            var get = _db.GetComponentDataModificaciones(squareId, agreementId, attachedId, relationShip, relationShipPrincipal);                            
            return Ok(get);
        }

        // GET: api/Component
        [HttpGet("{convenio}/{plaza}/{Id}/{marca}")]
        public ActionResult<Response> GetComponents(string convenio, string plaza, string Id, string marca)
        {
            var get = _db.GetComponentData(convenio, plaza, Id, marca);
            if(get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        //GET: api/Component/5
        [HttpGet("{idAgreement}")]
        public ActionResult<Response> Get(int idAgreement)
        {
            var get = _db.GetComponentsData(idAgreement);
            if(get.Result == null)
                return NotFound(get);
            return Ok(get);
        }
        //GET: api/Component/5
        [HttpGet("versionProduccion/{plaza}/{convenio}")]
        public ActionResult<Response> Get(string plaza, string convenio)
        {
            var get = _db.VersionPruebaComponet(plaza, convenio);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("Inventario/{plaza}")]
        public ActionResult<Response> GetComponentsInventory(string plaza)
        {
            var get = _db.GetComponentsInventory(plaza);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("InventarioUbicacion")]
        public ActionResult<Response> GetComponentsInventoryUbication()
        {
            var get = _db.GetComponentsInventoryUbication();
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("Inventario/{componente}/{plaza}")]
        public ActionResult<Response> GetComponentsInventoryLane(string componente, string plaza)
        {
            var get = _db.GetComponentsInventoryLane(componente, plaza);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("Inventario/{componente}/{linea}/{plaza}")]
        public ActionResult<Response> GetComponentsInventoryDescription(string componente, string linea, string plaza)
        { 
            var get = _db.GetComponentsInventoryDescription(componente, linea, plaza);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }
  
        // PUT: api/Component/5
        [HttpPut("updateInventory")]
        public ActionResult<Response> Put([FromBody] UpdateInventory updateInventory)
        {
            var put = _db.PutComponentInventary(updateInventory);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }


        [HttpPut("updateInventoryList")]
        public ActionResult<Response> Put([FromBody] List<ComponentsInventoryList> componentsInventoryList)
        {
            var put = _db.InventoryListUpdate(componentsInventoryList);
            if (put.Result == null)
                return NotFound(put);
            return Ok(put);

        }

    }
}
