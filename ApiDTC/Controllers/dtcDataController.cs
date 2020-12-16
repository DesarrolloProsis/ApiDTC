namespace ApiDTC.Controllers
{
    using System;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class DtcDataController : ControllerBase
    {
        #region Attributes
        private readonly DtcDataDb _db;
        #endregion

        #region Constructor
        public DtcDataController(DtcDataDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        #endregion

        #region Methods
        
        //[HttpGet("{IdUser}/{SquareCatalog}")]
        [HttpGet("{clavePlaza}/{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> Get(string clavePlaza, int IdUser, string SquareCatalog)
        {   
            var get = _db.GetDTC(clavePlaza, IdUser, SquareCatalog);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //[HttpGet("Open/{IdUser}/{SquareCatalog}")]
        [HttpGet("Open/{clavePlaza}/{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> GetOpen(string clavePlaza, int IdUser, string SquareCatalog)
        {
            var get = _db.GetDTC(clavePlaza, IdUser, SquareCatalog);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //[HttpGet("TableForm/{refNum}")]
        [HttpGet("TableForm/{clavePlaza}/{refNum}")]
        public ActionResult<Response> Ge(string clavePlaza, string refNum)
        {
            var get = _db.GetTableForm(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //[HttpGet("EditInfo/{refNum}")]
        [HttpGet("EditInfo/{clavePlaza}/{refNum}")]
        public ActionResult<Response> GetEditInfo(string clavePlaza, string refNum)
        {
            var get = _db.EditReferece(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("EditInfo/Open/{clavePlaza}/{refNum}")]
        public ActionResult<Response> GetEditInfoOpen(string clavePlaza, string refNum)
        {
            var get = _db.EditRefereceOpen(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("{clavePlaza}/{refNum}")]
        public ActionResult<Response> GetDtcData(string clavePlaza, string refNum)
        {
            var get = _db.GetReferenceNumber(clavePlaza, refNum);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //[HttpGet("InvalidReferenceNumbers")]
        [HttpGet("InvalidReferenceNumbers/{clavePlaza}")]
        public ActionResult<Response> GetInvalidReferenceNumbers(string clavePlaza)
        {
            var get = _db.GetInvalidNumbers(clavePlaza);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //[HttpGet("InventoryComponentsList/{squareCatalog}")]
        [HttpGet("InventoryComponentsList/{clavePlaza}/{squareCatalog}")]
        public ActionResult<Response> GetComponentsInventoryList(string clavePlaza, string squareCatalog)
        {
            var get = _db.GetComponentsInventoryList(clavePlaza, squareCatalog);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        //[HttpPost]
        [HttpPost("{clavePlaza}")]
        public ActionResult<Response> Post(string clavePlaza, [FromBody] DtcData dtcData)
        {
            if(ModelState.IsValid)
            {
                var get = _db.GetStoredDtcData(clavePlaza, dtcData);
                if(get.SqlResult == null)
                    return BadRequest(get);
                else
                    return StatusCode(201, get);    
            }
            return BadRequest(ModelState);
        }

        //[HttpDelete("Delete/{referenceNumber}")]
        [HttpDelete("Delete/{clavePlaza}/{referenceNumber}/{userId}")]
        public ActionResult<Response> Delete(string clavePlaza, string referenceNumber, int userId)
        {
            if (ModelState.IsValid)
            {
                var delete = _db.DeleteDtcData(clavePlaza, referenceNumber, userId);
                if (delete.SqlResult == null)
                    return NotFound(delete);
                else
                    return Ok(delete);
            }
            return BadRequest(ModelState);
        }

        //[HttpPut("UpdateStatus/{referenceNumber}")]
        [HttpPut("UpdateStatus/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> Update(string clavePlaza, string referenceNumber)
        {
            var put = _db.UpdateDtcStatus(clavePlaza, referenceNumber);
            if(put.Result == null)
                return NotFound(put);
            return Ok(put);
        }

        //NuevaAlex
        [HttpGet("{clavePlaza}/{ReferenceNumber}")]
        public ActionResult<Response> GetHeaderEdit (string clavePlaza, string ReferenceNumber)
        {
            var get = _db.GetDTCHeaderEdit(clavePlaza, ReferenceNumber);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }



        #endregion
    }
}
