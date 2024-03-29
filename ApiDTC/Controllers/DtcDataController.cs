﻿namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using ApiDTC.Models.Logs;
    using ApiDTC.Services;
    using Aspose.Imaging;
    using Aspose.Imaging.ImageOptions;
    using ClosedXML.Excel;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DtcDataController : ControllerBase
    {
        #region Attributes
        private readonly DtcDataDb _db;

        private readonly ApiLogger _apiLogger;

        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor
        public DtcDataController(DtcDataDb db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods

        #region Operaciones DTC
        //TODO mandar si tiene pdf y fotos
        [HttpGet("{clavePlaza}/{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> Get(string clavePlaza, int IdUser, string SquareCatalog)
        {
            var get = _db.GetDTC(clavePlaza, IdUser, SquareCatalog, this._disk, this._folder);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("Open/{clavePlaza}/{IdUser}/{SquareCatalog}")]
        public ActionResult<Response> GetOpen(string clavePlaza, int IdUser, string SquareCatalog)
        {
            var get = _db.GetDTC(clavePlaza, IdUser, SquareCatalog, this._disk, this._folder);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("TableForm/{clavePlaza}/{refNum}")]
        public ActionResult<Response> Ge(string clavePlaza, string refNum)
        {
            var get = _db.GetTableForm(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("EditInfo/{clavePlaza}/{refNum}")]
        public ActionResult<Response> GetEditInfo(string clavePlaza, string refNum)
        {
            var get = _db.EditReferece(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("EditInfo/Open/{clavePlaza}/{refNum}")]
        public ActionResult<Response> GetEditInfoOpen(string clavePlaza, string refNum)
        {
            var get = _db.EditRefereceOpen(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("BuscarReferencia/{clavePlaza}/{refNum}")]
        public ActionResult<Response> GetDtcData(string clavePlaza, string refNum)
        {
            var get = _db.GetReferenceNumber(clavePlaza, refNum);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("InvalidReferenceNumbers/{clavePlaza}")]
        public ActionResult<Response> GetInvalidReferenceNumbers(string clavePlaza)
        {
            var get = _db.GetInvalidNumbers(clavePlaza);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("InventoryComponentsList/{clavePlaza}/{squareId}/{CapufeNum}/{IdGare}")]
        public ActionResult<Response> GetComponentsInventoryList(string clavePlaza, string squareId, string CapufeNum, string IdGare)
        {
            //string clavePlaza, string squareId, string CapufeNum, string IdGare
            var get = _db.GetComponentsInventoryList(clavePlaza, squareId, CapufeNum, IdGare);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpPost("{clavePlaza}")]
        public ActionResult<Response> Post(string clavePlaza, [FromBody] DtcData dtcData)
        {
            if (ModelState.IsValid)
            {
                var get = _db.GetStoredDtcData(clavePlaza, dtcData);
                if (get.SqlResult == null)
                    return NotFound(get);
                return StatusCode(201, get);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("Delete/{clavePlaza}/{referenceNumber}/{userId}/{Comment}")]
        public ActionResult<Response> Delete(string clavePlaza, string referenceNumber, int userId, string Comment)
        {
            if (ModelState.IsValid)
            {
                var delete = _db.DeleteDtcData(clavePlaza, referenceNumber, userId, Comment);
                if (delete.SqlResult == null)
                    return NotFound(delete);
                return Ok(delete);
            }
            return BadRequest(ModelState);
        }

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
        public ActionResult<Response> GetHeaderEdit(string clavePlaza, string ReferenceNumber)
        {
            var get = _db.GetDTCHeaderEdit(clavePlaza, ReferenceNumber);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("GetReferencesLog")]
        public ActionResult<Response> GetReferencesLog()
        {
            var get = _db.GetReferencesLog();
            if (get.Result == null)
                return NotFound();
            return Ok(get);
        }

        [HttpGet("GetReferencesLogDetail/{ReferenceNumber}")]
        public ActionResult<Response> GetReferencesLogDetail(string ReferenceNumber)
        {
            var get = _db.GetReferencesLogDetails(ReferenceNumber);
            if (get.Result == null)
                return NotFound();
            return Ok(get);
        }
        #endregion

        #region Equipo Dañado
        [HttpPost("EquipoDañado/Images/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> InsertImageDaniado(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string referenceNumber)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs";
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
                    using (FileStream fs = new FileStream(Path.Combine(dir, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();

                        FileInfo fi = new FileInfo(Path.Combine(dir, filename));
                        if (fi.Length > 1000000)
                        {
                            string temporal = Path.Combine(dir, filename) + "_temp";
                            using (var imgOrigin = Image.Load(Path.Combine(dir, filename)))
                            {
                                var jpegOptions = new JpegOptions()
                                {
                                    CompressionType = Aspose.Imaging.FileFormats.Jpeg.JpegCompressionMode.Progressive
                                };
                                imgOrigin.Save(Path.Combine(dir, temporal), jpegOptions);
                            }
                            if (System.IO.File.Exists(Path.Combine(dir, filename)))
                            {
                                //Se borra archivo grande
                                System.IO.File.Delete(Path.Combine(dir, filename));
                                //Archivo temporal actualiza su nombre al real
                                System.IO.File.Move(Path.Combine(dir, temporal), Path.Combine(dir, filename));
                            }
                        }
                    }

                    return Ok(Path.Combine(dir, filename));
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "ImageController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
            }
            else
                return NotFound("Insert another image");
        }

        [AllowAnonymous]
        [HttpGet("EquipoDañado/Images/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadEquipoDaniadoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs\{fileName}";
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
                string directoy = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs";
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

        [AllowAnonymous]
        [HttpGet("EquipoDañado/Images/DeleteImg/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<string> DeleteEquipoDaniadoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs").Length == 0)
                    Directory.Delete($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoDañadoImgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: DeleteEquipoDaniadoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpPut("UpdateFechaDtc/{clavePlaza}")]
        public ActionResult<Response> UpdateFechaDtc(string clavePlaza, [FromBody] DtcFechas dtcData)
        {
            var get = _db.UpdateFechasDTC(clavePlaza, dtcData);
            if (get.Result == null)
                return NotFound();
            return Ok(get);
        }
        [HttpPut("UpdateDtcDFReference/{clavePlaza}/{DtcReference}/{DiagnosisReference}")]
        public ActionResult<Response> UpdateDTCReference(string clavePlaza, string DtcReference, string DiagnosisReference)
        {
            var get = _db.UpdateDTCDFReference(clavePlaza, DtcReference, DiagnosisReference);
            if (get.Result == null)
                return NotFound();
            return Ok(get);
        }
        [HttpGet("GetDTCNoSellado/")]
        public ActionResult<Response> GetListDTCNoSellados()
        {
            var get = _db.GetListDTCNoSellados();
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("DescargarExcelDTCNoSellado/")]
        public IActionResult DescargarExcelDTCNoSellado()
        {
            var get = _db.GetListDTCNoSellados();
            if (get.Result == null)
            {
                return null;
            }
            else
            {
                List<DTCNoSellado> lista = (List<DTCNoSellado>)get.Result;
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        int fila = 2;
                        var ws = workbook.Worksheets.Add("Lista");
                        ws.Cell(1, 1).Value = "ReferenceNumber";
                        ws.Cell(1, 2).Value = "StatusId";
                        ws.Cell(1, 3).Value = "FechaIngreso";
                        foreach (DTCNoSellado item in lista)
                        {
                            ws.Cell(fila, 1).Value = item.ReferenceNumber;
                            ws.Cell(fila, 2).Value = item.StatusId;
                            ws.Cell(fila, 3).Value = item.FechaIngreso;
                            fila++;

                        }
                        workbook.SaveAs("D:\\LDTCNoSellado.xlsx");


                        //return File(new FileStream("D:\\HelloWorld.xlsx", FileMode.Open, FileAccess.Read), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        byte[] bytes = System.IO.File.ReadAllBytes("D:\\LDTCNoSellado.xlsx");
                        return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LDTCNoSellado.xlsx");
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }            
        }
        
        [HttpGet("DescargarExcelBorradosDTC/")]
        public IActionResult DescargarExcelBorradosDTC()
        {
            var get = _db.GetReferencesLog();
            if (get.Result == null)
            {
                return null;
            }
            else
            {
                List<ReferenceLog> lista = (List<ReferenceLog>)get.Result;
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        int fila = 2;
                        var ws = workbook.Worksheets.Add("Lista");
                        ws.Cell(1, 1).Value = "Numero Referencia";
                        ws.Cell(1, 2).Value = "Ultima Fecha";
                        ws.Cell(1, 3).Value = "Conteos";                                                                                                                        
                        foreach (ReferenceLog item in lista)
                        {
                            ws.Cell(fila, 1).Value = item.RefereceNumber;
                            ws.Cell(fila, 2).Value = item.UltimaFecha.ToShortDateString();
                            ws.Cell(fila, 3).Value = item.Conteos;                                                                                                                                            
                            fila++;
                        }
                        workbook.SaveAs("D:\\DtcDeleteLog.xlsx");
                        //return File(new FileStream("D:\\HelloWorld.xlsx", FileMode.Open, FileAccess.Read), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        byte[] bytes = System.IO.File.ReadAllBytes("D:\\DtcDeleteLog.xlsx");
                        return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "DtcDeleteLog.xlsx");
                    }
                }
                catch (Exception)
                {
                    return null;
                }
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
                string dir = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs";
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
                    using (FileStream fs = new FileStream(Path.Combine(dir, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();

                        FileInfo fi = new FileInfo(Path.Combine(dir, filename));
                        if (fi.Length > 1000000)
                        {
                            string temporal = Path.Combine(dir, filename) + "_temp";
                            using (var imgOrigin = Image.Load(Path.Combine(dir, filename)))
                            {
                                var jpegOptions = new JpegOptions()
                                {
                                    CompressionType = Aspose.Imaging.FileFormats.Jpeg.JpegCompressionMode.Progressive
                                };
                                imgOrigin.Save(Path.Combine(dir, temporal), jpegOptions);
                            }
                            if (System.IO.File.Exists(Path.Combine(dir, filename)))
                            {
                                //Se borra archivo grande
                                System.IO.File.Delete(Path.Combine(dir, filename));
                                //Archivo temporal actualiza su nombre al real
                                System.IO.File.Move(Path.Combine(dir, temporal), Path.Combine(dir, filename));
                            }
                        }
                    }
                    return Ok(Path.Combine(dir, filename));
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
            }
            else
                return NotFound("Insert another image");
        }

        [AllowAnonymous]
        [HttpGet("EquipoNuevo/Images/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadEquipoNuevoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs\{fileName}";
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
                string directoy = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs";
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

        [AllowAnonymous]
        [HttpGet("EquipoNuevo/Images/DeleteImg/{clavePlaza}/{referenceNumber}/{fileName}")]
        public ActionResult<string> DeleteEquipoNuevoImg(string clavePlaza, string referenceNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs").Length == 0)
                    Directory.Delete($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\EquipoNuevoImgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DtcDataController: DeleteEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpPut("UpdateUserIdOfDTC/{ClavePlaza}/{userId}/{referenceNumberDtc}/{referenceDiag}")]
        public ActionResult<Response> UpdateUserIdOfDTC(string ClavePlaza, int userId, string referenceNumberDtc, string referenceDiag)
        {
            if (ModelState.IsValid)
            {
                var put = _db.UpdateUserIdOfDTC(ClavePlaza, userId, referenceNumberDtc, referenceDiag);
                if (put.Result == null)
                    return NotFound(put);
                return Ok(put);
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("ListarDTCBorrado/{ClavePlaza}")]
        public ActionResult<Response> GetDTCBorrado(string ClavePlaza)
        {
            //var get = _db.GetDTCBorrado(ClavePlaza);
            //if (get.Result == null)
            //    return NotFound(get);
            //return Ok(get);
            var get = _db.GetDTCBorrado(ClavePlaza);
            if (get.Result == null)
            {
                return null;
            }
            else
            {
                List<DTCBorrado> lista = (List<DTCBorrado>)get.Result;
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        int fila = 2;
                        var ws = workbook.Worksheets.Add("Lista");
                        ws.Cell(1, 1).Value = "Numero de referencia";
                        ws.Cell(1, 2).Value = "Conteos";
                        ws.Cell(1, 3).Value = "Ultima fecha";
                        ws.Cell(1, 4).Value = "Usuario";
                        ws.Cell(1, 5).Value = "Comentario";
                        foreach (DTCBorrado item in lista)
                        {
                            ws.Cell(fila, 1).Value = item.RefereceNumber;
                            ws.Cell(fila, 2).Value = item.Conteos;
                            ws.Cell(fila, 3).Value = item.UltimaFecha;
                            ws.Cell(fila, 4).Value = item.UserName;
                            ws.Cell(fila, 5).Value = item.Comment;
                            fila++;

                        }
                        workbook.SaveAs("D:\\DTCBorrado.xlsx");


                        //return File(new FileStream("D:\\HelloWorld.xlsx", FileMode.Open, FileAccess.Read), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        byte[] bytes = System.IO.File.ReadAllBytes("D:\\DTCBorrado.xlsx");
                        return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "DTCBorrado.xlsx");
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }
        #endregion
        #endregion
    }
}
