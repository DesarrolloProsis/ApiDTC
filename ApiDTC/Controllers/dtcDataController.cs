namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class DtcDataController : ControllerBase
    {
        private readonly DtcDataDb _db;
        public DtcDataController(DtcDataDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }

        //Agregar imágenes
        //GET: api/dtcData
        [HttpGet("{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> Get(int IdUser, string SquareCatalog)
        {   
            var get = _db.GetDTC(IdUser, SquareCatalog);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("Open/{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> GetOpen(int IdUser, string SquareCatalog)
        {
            var get = _db.GetDTC(IdUser, SquareCatalog);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("TableForm/{refNum}")]
        public ActionResult<Response> Ge(string refNum)
        {
            var get = _db.GetTableForm(refNum);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("EditInfo/{refNum}")]
        public ActionResult<Response> GetEditInfo(string refNum)
        {
            var get = _db.EditReferece(refNum);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("EditInfo/Open/{refNum}")]
        public ActionResult<Response> GetEditInfoOpen(string refNum)
        {
            var get = _db.EditRefereceOpen(refNum);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("{refNum}")]
        public ActionResult<Response> GetDtcData(string refNum)
        {
            var get = _db.GetReferenceNumber(FormatUtil.ReferenceFormat(refNum));
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }
        [HttpGet("InvalidReferenceNumbers")]
        public ActionResult<Response> GetInvalidReferenceNumbers()
        {
            var get = _db.GetInvalidNumbers();
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }


        [HttpGet("InventoryComponentsList/{squareCatalog}")]
        public ActionResult<Response> GetComponentsInventoryList(string squareCatalog)
        {
            var get = _db.GetComponentsInventoryList(squareCatalog);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }



        //TODO Ajustar petición POST nuevo DTC
        // POST: api/dtcData/NuevoDtc
        [HttpPost]
        public ActionResult<Response> Post([FromBody] DtcData dtcData)
        {
            if(ModelState.IsValid)
            {
                var get = _db.GetStoredDtcData(dtcData);
                if(get.SqlResult == null)
                    return BadRequest(get);
                else
                    return StatusCode(201, get);    
                    //return Created($"api/dtcdata/{dtcData.ReferenceNumber}", dtcData);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("Delete/{referenceNumber}")]
        public ActionResult<Response> Delete(string referenceNumber)
        {
            if (ModelState.IsValid)
            {
                var delete = _db.DeleteDtcData(referenceNumber);
                if (delete.SqlResult == null)
                    return NotFound(delete);
                else
                    return Ok(delete);
            }
            return BadRequest(ModelState);
        }
    }
}
