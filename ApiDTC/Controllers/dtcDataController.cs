namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class DtcDataController : ControllerBase
    {
        #region Attributes
        private readonly DtcDataDb _db;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructor
        public DtcDataController(DtcDataDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods

        #region Operaciones DTC
        [HttpGet("{clavePlaza}/{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> Get(string clavePlaza, int IdUser, string SquareCatalog)
        {   
            var get = _db.GetDTC(clavePlaza, IdUser, SquareCatalog);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("Open/{clavePlaza}/{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> GetOpen(string clavePlaza, int IdUser, string SquareCatalog)
        {
            var get = _db.GetDTC(clavePlaza, IdUser, SquareCatalog);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("TableForm/{clavePlaza}/{refNum}")]
        public ActionResult<Response> Ge(string clavePlaza, string refNum)
        {
            var get = _db.GetTableForm(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

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

        [HttpGet("BuscarReferencia/{clavePlaza}/{refNum}")]
        public ActionResult<Response> GetDtcData(string clavePlaza, string refNum)
        {
            var get = _db.GetReferenceNumber(clavePlaza, refNum);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("InvalidReferenceNumbers/{clavePlaza}")]
        public ActionResult<Response> GetInvalidReferenceNumbers(string clavePlaza)
        {
            var get = _db.GetInvalidNumbers(clavePlaza);
            if(get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

        [HttpGet("InventoryComponentsList/{clavePlaza}/{squareCatalog}")]
        public ActionResult<Response> GetComponentsInventoryList(string clavePlaza, string squareCatalog)
        {
            var get = _db.GetComponentsInventoryList(clavePlaza, squareCatalog);
            if (get.Result == null)
                return NotFound(get);
            else
                return Ok(get);
        }

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

        //[HttpPut("UpdateStatus/{clavePlaza}/{referenceNumber}")]
        /*public ActionResult<Response> Update(string clavePlaza, string referenceNumber)
        {
            var put = _db.UpdateDtcStatus(clavePlaza, referenceNumber);
            if(put.Result == null)
                return NotFound(put);
            return Ok(put);
        }
        */
        [HttpPut("UpdateDtcHeader/{clavePlaza}")]
        public ActionResult<Response> UpdateHeadDtc(string clavePlaza, [FromBody] DtcHeader dtcHeader)
        {
            if (ModelState.IsValid)
            {
                var put = _db.UpdateDtcHeader(clavePlaza, dtcHeader);
                if (put.Result == null)
                    return NotFound(put);
                return Ok(put);
            }
            return BadRequest();
        }

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
        
        #region Equipo Dañado
        [HttpPost("EquipoDañado/Images/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> InsertImageDaniado(string clavePlaza, [FromForm(Name = "image")] IFormFile image,  string referenceNumber)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs";
                string filename;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{referenceNumber}_EquipoDañadoImg_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{referenceNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    var fs = new FileStream(Path.Combine(dir, filename), FileMode.Create);
                    image.CopyTo(fs);
                    fs.Close();
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "ImageController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
                return Ok(Path.Combine(dir, filename));
            }
            else
                return NotFound("Insert another image");
        }

        [HttpGet("EquipoDañado/Images/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadEquipoDaniadoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: DownloadEquipoDaniadoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("EquipoDañado/Images/GetPaths/{clavePlaza}/{referenceNumber}")]
        public ActionResult<List<string>> GetImagesEquipoDaniado(string clavePlaza, string referenceNumber)
        {
            try
            {
                string directoy = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: GetImagesEquipoDaniado", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("EquipoDañado/Images/DeleteImg/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<string> DeleteEquipoDaniadoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs").Length == 0)
                    Directory.Delete($@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: DeleteEquipoDaniadoImg", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion

        #region Equipo Nuevo
        [HttpPost("EquipoNuevo/Images/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> InsertImageNuevo(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string referenceNumber)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs";
                string filename;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{referenceNumber}_EquipoNuevoImg_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{referenceNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    var fs = new FileStream(Path.Combine(dir, filename), FileMode.Create);
                    image.CopyTo(fs);
                    fs.Close();
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "ImageController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
                return Ok(Path.Combine(dir, filename));
            }
            else
                return NotFound("Insert another image");
        }

        [HttpGet("EquipoNuevo/Images/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadEquipoNuevoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: DownloadEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("EquipoNuevo/Images/GetPaths/{clavePlaza}/{referenceNumber}")]
        public ActionResult<List<string>> GetImagesEquipoNuevo(string clavePlaza, string referenceNumber)
        {
            try
            {
                string directoy = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: GetImagesEquipoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("EquipoNuevo/Images/DeleteImg/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<string> DeleteEquipoNuevoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs").Length == 0)
                    Directory.Delete($@"C:\Bitacora\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: DeleteEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion
        #endregion
    }
}
