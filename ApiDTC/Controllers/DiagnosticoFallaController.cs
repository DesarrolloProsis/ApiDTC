﻿namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.IO;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using ApiDTC.Services;
    using Aspose.Imaging;
    using Aspose.Imaging.ImageOptions;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DiagnosticoFallaController : ControllerBase
    {
        #region Attributes
        private readonly DiagnosticoDb _db;

        private readonly ApiLogger _apiLogger;

        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor
        public DiagnosticoFallaController(DiagnosticoDb db, IConfiguration configuration)
        {

            this._db = db ?? throw new ArgumentNullException(nameof(db));
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            _apiLogger = new ApiLogger();
        }
        #endregion

        //https://localhost:44358/api/DiagnosticoFalla/TLA/B01/TLA-DF-001-02
        #region Methods
        //GET DIAGNÓSTICO DE FALLA
        #region PDF
        [HttpGet("{clavePlaza}/{referenceNumber}")]
        public IActionResult GetDiagnosticoFalla(string clavePlaza, string referenceNumber)
        {
            try
            {
                var getInfoPdf = _db.GetDiagnosticoInfoPdf(clavePlaza, referenceNumber);
                if (getInfoPdf == null)
                    return NotFound();
                DiagnosticoFallaPdfCreation pdf = new DiagnosticoFallaPdfCreation(clavePlaza, getInfoPdf, new ApiLogger());
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "FichaTecnicaAtencionController: GetFichaTecnicaAtencion", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Exists/{clavePlaza}/{referenceNumber}")]
        public ActionResult DiagnosticoFallaExists(string clavePlaza, string referenceNumber)
        {
            string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\Reportes\{referenceNumber}\{referenceNumber}-DiagnosticoSellado.pdf";
            if (System.IO.File.Exists((path)))
                return Ok();
            return NotFound();
        }

        [HttpPost("Sellada/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> DiagnosticoFallaSellado(string clavePlaza, [FromForm(Name = "file")] IFormFile file, string referenceNumber)
        {
            if (file.Length > 0 || file != null)
            {
                if (file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\Reportes\{referenceNumber}", filename;
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        filename = $"{referenceNumber}-DiagnosticoSellado.pdf";
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        var fs = new FileStream(Path.Combine(path, filename), FileMode.Create);
                        file.CopyTo(fs);
                        fs.Close();
                        return Ok(path);
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: DiagnosticoFallaSellado", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        #endregion

        [HttpGet("GetDiagnosticoInfo/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> GetBitacora(string clavePlaza, string referenceNumber)
        {
            var get = _db.GetDiagnosticoInfo(clavePlaza, referenceNumber);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpGet("GetBitacoras/{clavePlaza}/{userId}")]
        public ActionResult<Response> GetDiagnosticos(string clavePlaza, int userId)
        {
            var get = _db.GetDiagnosticos(clavePlaza, userId, this._disk, this._folder);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        [HttpPost("InsertDiagnosticoDeFalla/{clavePlaza}")]
        public ActionResult<Response> InsertDiagnosticoDeFalla(string clavePlaza, [FromBody] DiagnosticoDeFalla diagnosticoDeFalla)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertFaultDiagnosis(clavePlaza, diagnosticoDeFalla);
                if (get.Result == null)
                    return BadRequest(get);
                else
                    return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("FichaTecnicaDiagnosticoLane/{clavePlaza}")]
        public ActionResult<Response> InsertFichaTecnicaIntervencionLane(string clavePlaza, [FromBody] FichaTecnicaIntervencionLane fichaTecnicaIntervencionLane)
        {
            if (ModelState.IsValid)
            {
                var get = _db.InsertFichaTecnicaIntervencionLane(clavePlaza, fichaTecnicaIntervencionLane);
                if (get.Result == null)
                    return BadRequest(get);
                return Ok(get);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetReference/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> GetReference(string clavePlaza, string referenceNumber)
        {
            var get = _db.GetReferenceNumberDiagnosis(clavePlaza, referenceNumber);
            if (get.Result == null)
                return NotFound(get);
            return Ok(get);
        }

        #region DiagnosticoImages
        [HttpPost("Images/{clavePlaza}/{reportNumber}")]
        public ActionResult<Response> InsertImageNuevo(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string reportNumber)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs";
                string dirFull = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgsFullSize";
                string filename;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    if (!Directory.Exists(dirFull))
                        Directory.CreateDirectory(dirFull);

                    if (Directory.GetFiles(dir).Length >= 4)
                        return NotFound("Ya existen cuatro imágenes");

                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{reportNumber}_DiagnosticoFallaImgs_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{reportNumber}_Image_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }
                    //full
                    using (FileStream fs = new FileStream(Path.Combine(dirFull, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();
                        //if (fi.Length > 1000000)
                        //{
                        //}
                    }
                    //full

                    using (FileStream fs = new FileStream(Path.Combine(dir, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();

                        FileInfo fi = new FileInfo(Path.Combine(dir, filename));
                        //if(fi.Length > 1000000)
                        //{
                        string temporal = Path.Combine(dir, filename) + "_temp";
                        this.VaryQualityLevel(Path.Combine(dir, filename), temporal);
                        //using(var imgOrigin = Image.Load(Path.Combine(dir, filename)))
                        //{
                        //    var jpegOptions = new JpegOptions(){
                        //        CompressionType = Aspose.Imaging.FileFormats.Jpeg.JpegCompressionMode.Progressive
                        //    };
                        //    imgOrigin.Save(Path.Combine(dir, temporal), jpegOptions);
                        //}
                        if (System.IO.File.Exists(Path.Combine(dir, filename)))
                        {
                            //Se borra archivo grande
                            System.IO.File.Delete(Path.Combine(dir, filename));
                            //Archivo temporal actualiza su nombre al real
                            System.IO.File.Move(Path.Combine(dir, temporal), Path.Combine(dir, filename));
                        }
                        //}
                    }
                    return Ok(Path.Combine(dir, filename));
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: InsertImage", 2);
                    return NotFound(ex.ToString());
                }
            }
            else
                return NotFound("Insert another image");
        }

        public void VaryQualityLevel(string fileName, string fileTemporal)
        {
            // Get a bitmap.
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(fileName);
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(bmp1, 300, 300);
            bmp2.Save(fileTemporal, jgpEncoder,
                myEncoderParameters);
            bmp1.Dispose();


        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }



        [AllowAnonymous]
        [HttpGet("Images/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadDiagnosticoImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: DownloadDiagnosticoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Images/GetPaths/{clavePlaza}/{reportNumber}")]
        public ActionResult<List<string>> GetImagesDiagnostico(string clavePlaza, string reportNumber)
        {
            try
            {
                string directoy = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: GetImagesDiagnostico", 2);
                return NotFound(ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpGet("Images/DeleteImg/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<string> DeleteDiagnosticoImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs").Length == 0)
                    Directory.Delete($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\DiagnosticoFallaImgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "DiagnosticoFallaController: DeleteDiagnosticoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        //Borrar Diagnostico
        //[AllowAnonymous]
        [HttpPost("BorraDiagnosticoFull/{clavePlaza}/{ReferenceNumber}/{UserId}/{Comment}/{ReferenceDTC}")]
        public ActionResult<Response> BorraDiagnosticoFull(string clavePlaza, string ReferenceNumber, int UserId, string Comment, string ReferenceDTC)
        {
            if (ModelState.IsValid)
            {
                var get = _db.BorraDiagnosticoFull(clavePlaza, ReferenceNumber, UserId, Comment, ReferenceDTC);
                if (get.Result == null)
                {
                    return BadRequest(get);
                }
                else
                {
                    borraArchivosDiagnostico(clavePlaza, ReferenceNumber, ReferenceDTC);
                    return Ok(get);
                }


            }
            return BadRequest(ModelState);
        }

        //[AllowAnonymous]
        //[HttpPost("BorraFoldersFull/{clavePlaza}/{ReferenceNumber}/{ReferenceDTC}")]
        public void borraArchivosDiagnostico(string clavePlaza, string ReferenceNumber, string ReferenceDTC)
        {
            try
            {
                string fechaDel = armaFecha();
                string pathReporte = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{ReferenceNumber}";
                string pathDTC = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{ReferenceDTC}";
                if (crearEstructuraDel())
                {
                    if (checkIfExist(pathReporte))
                    {
                        string pathReporteDel = $@"{this._disk}:\{this._folder}\borrado\Reportes\{ReferenceNumber}-" + armaFecha();
                        Directory.Move(pathReporte, pathReporteDel);
                    }
                    if (checkIfExist(pathDTC) && !ReferenceDTC.Equals("--"))
                    {
                        string pathDTCDel = $@"{this._disk}:\{this._folder}\borrado\DTC\{ReferenceDTC}-" + armaFecha();
                        Directory.Move(pathDTC, pathDTCDel);
                    }
                }

                Console.WriteLine(pathReporte + "--" + pathDTC);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string armaFecha()
        {
            string fechaBorrado = DateTime.Now.ToString().Replace(" ", "");
            fechaBorrado = fechaBorrado.Replace("/", "");
            fechaBorrado = fechaBorrado.Replace(":", "");
            return fechaBorrado;
        }

        public bool checkIfExist(string ruta)
        {
            if (Directory.Exists(ruta))
            {
                return true;
            }
            return false;
        }

        public bool crearEstructuraDel()
        {
            string pathBorraReporte = $@"{this._disk}:\{this._folder}\borrado\Reportes";
            string pathBorraDTC = $@"{this._disk}:\{this._folder}\borrado\DTC";
            bool canCreate = false;
            if (Directory.Exists(pathBorraReporte))
            {
                canCreate = true;
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(pathBorraReporte);
                canCreate = true;
            }
            if (Directory.Exists(pathBorraDTC))
            {
                canCreate = true;
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(pathBorraDTC);
                canCreate = true;
            }
            return canCreate;
        }
        #endregion
        #endregion
    }
}