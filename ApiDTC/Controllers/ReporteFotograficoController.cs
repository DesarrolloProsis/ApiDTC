﻿namespace ApiDTC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiDTC.Data;
    using ApiDTC.Models;
    using Aspose.Imaging;
    using Aspose.Imaging.ImageOptions;
    using ApiDTC.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Data;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReporteFotograficoController : ControllerBase
    {
        #region Attributes
        private readonly ApiLogger _apiLogger;

        private ReporteFotograficoDB _db;

        private readonly string _disk;

        private readonly string _folder;
        #endregion

        #region Constructor
        public ReporteFotograficoController(ReporteFotograficoDB db, IConfiguration configuration)
        {
            this._disk = $@"{Convert.ToString(configuration.GetValue<string>("Path:Disk"))}";
            this._folder = $"{Convert.ToString(configuration.GetValue<string>("Path:Folder"))}";
            this._db = db ?? throw new ArgumentNullException(nameof(db)); 
            _apiLogger = new ApiLogger();
        }
        #endregion

        #region Methods

        #region Reporte fotográfico mantenimiento preventivo
        [HttpGet("Reporte/{clavePlaza}/{referenceNumber}/{ubicacion}")]
        public IActionResult GetReporteFotografico(string clavePlaza, string referenceNumber, string ubicacion)
        {
            try
            {
                var dataSet = _db.GetStorePDFReporteFotografico(clavePlaza, referenceNumber);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStoredPdf retorna tabla vacía");
                ReporteFotograficoPdfCreation pdf = new ReporteFotograficoPdfCreation(clavePlaza, dataSet.Tables[0], new ApiLogger(), 1, referenceNumber, ubicacion);
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteFotografico", 2);
                return NotFound(ex.ToString());
            }
            
        }

        [HttpGet("MantenimientoExists/{clavePlaza}/{referenceNumber}")]
        public ActionResult PdfExists(string clavePlaza, string referenceNumber)
        {
            string path =  $@"{this._disk}:\{this._folder}\{clavePlaza}\Reportes\{referenceNumber}\ReporteFotográficoSellado-{referenceNumber}.pdf";
            if(System.IO.File.Exists((path)))
                return Ok();
            return NotFound();
        }

        [HttpPost("MantenimiendoSellado/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> FichaTecnicaSellada(string clavePlaza, [FromForm(Name = "file")] IFormFile file, string referenceNumber)
        {
            if(file.Length > 0 || file != null)
            {
                if(file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\Reportes\{referenceNumber}", filename;
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        filename = $"ReporteFotográficoSellado-{referenceNumber}.pdf";
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        using (FileStream fs = new FileStream(Path.Combine(path, filename), FileMode.Create))
                        {
                            file.CopyTo(fs);
                            fs.Close();
                        }
                        return Ok(path);
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: MantenimiendoSellado", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        [HttpPost("MantenimientoPreventivo/Images/{clavePlaza}/{reportNumber}")]
        public ActionResult<Response> InsertImageNuevo(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string reportNumber, int semana)
        {
            if (image.Length > 0 || image != null)
            {
                int numberOfImages;
                string dir = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\Imgs";
                string dirFull = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\ImgsFullSize";
                string filename;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (!Directory.Exists(dirFull))
                        Directory.CreateDirectory(dirFull);

                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{reportNumber}_MantenimientoPreventivoImgs_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{reportNumber}_MantenimientoPreventivoImgs_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }

                    //full
                    using (FileStream fs = new FileStream(Path.Combine(dirFull, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();
                    }
                    //full
                    using (FileStream fs = new FileStream(Path.Combine(dir, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();

                        FileInfo fi = new FileInfo(Path.Combine(dir, filename));
                        string temporal = Path.Combine(dir, filename) + "_temp";
                        this.VaryQualityLevel(Path.Combine(dir, filename), temporal);
                        if (System.IO.File.Exists(Path.Combine(dir, filename)))
                        {
                            //Se borra archivo grande
                            System.IO.File.Delete(Path.Combine(dir, filename));
                            //Archivo temporal actualiza su nombre al real
                            System.IO.File.Move(Path.Combine(dir, temporal), Path.Combine(dir, filename));
                        }
                    }
                    return Ok(Path.Combine(dir, filename));
                }
                catch (IOException ex)
                {
                    _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: InsertImage", 2);
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
            System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(bmp1,   300, 300);
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
        [HttpGet("MantenimientoPreventivo/Images/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<DtcImage> DownloadEquipoNuevoImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\Imgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: DownloadEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("MantenimientoPreventivo/Images/GetPaths/{clavePlaza}/{reportNumber}")]
        public ActionResult<List<string>> GetImagesEquipoNuevo(string clavePlaza, string reportNumber)
        {
            try
            {
                string directoy = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\Imgs";
                List<string> dtcImages = new List<string>();
                if (!Directory.Exists(directoy))
                    return Ok(dtcImages);
                foreach (var item in Directory.GetFiles(directoy))
                    dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                return Ok(dtcImages);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetImagesEquipoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpGet("MantenimientoPreventivo/Images/DeleteImg/{clavePlaza}/{reportNumber}/{fileName}")]
        public ActionResult<string> DeleteEquipoNuevoImg(string clavePlaza, string reportNumber, string fileName)
        {
            try
            {
                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\Imgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\Imgs").Length == 0)
                    Directory.Delete($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\Reportes\{reportNumber}\Imgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: DeleteEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }
        #endregion

        #region Equipo nuevo y dañado
        [HttpGet("Nuevo/{clavePlaza}/{ubicacion}/{referenceNumber}")]
        public IActionResult GetReporteEqupoNuevo(string clavePlaza, string ubicacion, string referenceNumber)
        {
            try
            {
                var dataSet = _db.GetStorePDF(clavePlaza, referenceNumber);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStoredPdf retorna tabla vacía");
                //2 equipo nuevo
                //3 equipo dañado
                ReporteFotograficoPdfCreation pdf = new ReporteFotograficoPdfCreation(clavePlaza, dataSet.Tables[0], new ApiLogger(), 2, referenceNumber, ubicacion);
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");

            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteEqupoNuevo", 2);
                return NotFound(ex.ToString());
            }
        }
        
        [HttpGet("Nuevo/PdfExists/{clavePlaza}/{referenceNumber}")]
        public ActionResult NuevoPdfSelladoExists(string clavePlaza, string referenceNumber)
        {
            string path =  $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}\DTC-{referenceNumber}-EquipoNuevoSellado.pdf";
            if(System.IO.File.Exists((path)))
                return Ok();
            return NotFound();
        }

        [HttpPost("Nuevo/ReporteSellado/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> EquipoNuevoSellado(string clavePlaza, [FromForm(Name = "file")] IFormFile file, string referenceNumber)
        {
            if(file.Length > 0 || file != null)
            {
                if(file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}", filename;
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        filename = $"DTC-{referenceNumber}-EquipoNuevoSellado.pdf";
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        using (FileStream fs = new FileStream(Path.Combine(path, filename), FileMode.Create))
                        {
                            file.CopyTo(fs);
                            fs.Close();
                        }
                        return Ok(path);
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: EquipoNuevoSellado", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        //REPORTE FOTOGRAFICO DE EQUIPO DAÑADO
        [HttpGet("Dañado/{clavePlaza}/{ubicacion}/{referenceNumber}")]
        public IActionResult GetReporteEquipoDañado(string clavePlaza, string ubicacion, string referenceNumber)
         {
            try
            {
                var dataSet = _db.GetStorePDF(clavePlaza, referenceNumber);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStoredPdf retorna tabla vacía");
                ReporteFotograficoPdfCreation pdf = new ReporteFotograficoPdfCreation(clavePlaza, dataSet.Tables[0], new ApiLogger(), 3, referenceNumber, ubicacion);
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteEquipoDañado", 2);
                return NotFound(ex.ToString());
            }
        }

        [HttpGet("Dañado/PdfExists/{clavePlaza}/{referenceNumber}")]
        public ActionResult DañadoPdfSelladoExists(string clavePlaza, string referenceNumber)
        {
            string path =  $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}\DTC-{referenceNumber}-EquipoDañadoSellado.pdf";
            if(System.IO.File.Exists((path)))
                return Ok();
            return NotFound();
        }

        [HttpGet("Dañado/ReporteSellado/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> GetEquipoDañadoSellado(string clavePlaza, string referenceNumber)
        {
            string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}\DTC-{referenceNumber}-EquipoDañadoSellado.pdf";
            try
            {
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                return File(new FileStream(path, FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "PDFController: GetPdfFotograficoSellado", 2);
                return NotFound(ex.ToString());
            }
        }
        [HttpPost("Dañado/ReporteSellado/{clavePlaza}/{referenceNumber}")]
        public ActionResult<Response> EquipoDañadoSellado(string clavePlaza, [FromForm(Name = "file")] IFormFile file, string referenceNumber)
        {
            if(file.Length > 0 || file != null)
            {
                if(file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".PDF"))
                {
                    string path = $@"{this._disk}:\{this._folder}\{clavePlaza}\DTC\{referenceNumber}", filename;
                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        filename = $"DTC-{referenceNumber}-EquipoDañadoSellado.pdf";
                        if (System.IO.File.Exists(Path.Combine(path, filename)))
                            System.IO.File.Delete(Path.Combine(path, filename));
                        
                        using(FileStream fs = new FileStream(Path.Combine(path, filename), FileMode.Create))
                        {
                            file.CopyTo(fs);
                            fs.Close();
                        }
                        return Ok(path);
                    }
                    catch (IOException ex)
                    {
                        _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: EquipoDañadoSellado", 2);
                        return NotFound(ex.ToString());
                    }
                }
                return NotFound("Ingresa un archivo pdf");
            }
            return NotFound();
        }

        #region Anexo

        [AllowAnonymous]
        [HttpGet("Images/GetPaths/{clavePlaza}/{reportNumber}/{referenceAnexo}/{isSubAnexo}")]
        public ActionResult<List<string>> GetImagesNuevo(string clavePlaza, string reportNumber, string referenceAnexo, bool isSubAnexo)
        {
            try
            {
                var dataSet = _db.GetMostRecentAnexoReference(clavePlaza, referenceAnexo, isSubAnexo);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("spMostRecentSubAnexo retorna tabla vacía");
                DataTable TableAnexo = dataSet.Tables[0];

                string directoy = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{reportNumber}\Reportes Fotograficos Equipo Nuevo\{TableAnexo.Rows[0]["NewerReference"]}\Imgs";
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
        [HttpPost("EquipoNuevo/Images/{clavePlaza}/{reportNumber}/{referenceAnexo}/{isSubAnexo}")]
        public ActionResult<Response> InsertImageNuev(string clavePlaza, [FromForm(Name = "image")] IFormFile image, string reportNumber, string referenceAnexo, bool isSubAnexo)
        {
            try
            {
                var dataSet = _db.GetMostRecentAnexoReference(clavePlaza, referenceAnexo, isSubAnexo);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("spMostRecentSubAnexo retorna tabla vacía");
                DataTable TableAnexo = dataSet.Tables[0];

                if (image.Length > 0 || image != null)
                {
                    int numberOfImages;
                    string dir = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{reportNumber}\Reportes Fotograficos Equipo Nuevo\{TableAnexo.Rows[0]["NewerReference"]}\Imgs";
                    string dirFull = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{reportNumber}\Reportes Fotograficos Equipo Nuevo\{TableAnexo.Rows[0]["NewerReference"]}\ImgsFullSize";
                    string filename;

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (!Directory.Exists(dirFull))
                        Directory.CreateDirectory(dirFull);

                    numberOfImages = Directory.GetFiles(dir).Length + 1;
                    filename = $"{reportNumber}_EquipoNuevoImgs_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    while (System.IO.File.Exists(Path.Combine(dir, filename)))
                    {
                        numberOfImages += 1;
                        filename = $"{reportNumber}_EquipoNuevoImgs_{numberOfImages}{image.FileName.Substring(image.FileName.LastIndexOf('.'))}";
                    }

                    //full
                    using (FileStream fs = new FileStream(Path.Combine(dirFull, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();
                    }
                    //full
                    using (FileStream fs = new FileStream(Path.Combine(dir, filename), FileMode.Create))
                    {
                        image.CopyTo(fs);
                        fs.Close();

                        FileInfo fi = new FileInfo(Path.Combine(dir, filename));
                        string temporal = Path.Combine(dir, filename) + "_temp";
                        this.VaryQualityLevel(Path.Combine(dir, filename), temporal);
                        if (System.IO.File.Exists(Path.Combine(dir, filename)))
                        {
                            //Se borra archivo grande
                            System.IO.File.Delete(Path.Combine(dir, filename));
                            //Archivo temporal actualiza su nombre al real
                            System.IO.File.Move(Path.Combine(dir, temporal), Path.Combine(dir, filename));
                        }
                    }
                    return Ok(Path.Combine(dir, filename));

                }
                else
                    return NotFound("Insert another image");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: InsertImage", 2);
                return NotFound(ex.ToString());
            }
            
        }

        [AllowAnonymous]
        [HttpGet("EquipoNuevo/Images/{clavePlaza}/{reportNumber}/{fileName}/{referenceAnexo}/{isSubAnexo}")]
        public ActionResult<DtcImage> DownloadEquipoNuevoImgs(string clavePlaza, string reportNumber, string fileName, string referenceAnexo, bool isSubAnexo)
        {
            try
            {
                var dataSet = _db.GetMostRecentAnexoReference(clavePlaza, referenceAnexo, isSubAnexo);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("spMostRecentSubAnexo retorna tabla vacía");
                DataTable TableAnexo = dataSet.Tables[0];

                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{reportNumber}\Reportes Fotograficos Equipo Nuevo\{TableAnexo.Rows[0]["NewerReference"]}\Imgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound("No existe el archivo");
                Byte[] bitMap = System.IO.File.ReadAllBytes(path);

                return File(bitMap, "Image/jpg");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: DownloadEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpGet("EquipoNuevo/Images/DeleteImg/{clavePlaza}/{reportNumber}/{fileName}/{referenceAnexo}/{isSubAnexo}")]
        public ActionResult<string> DeleteEquipoNuevoImgs(string clavePlaza, string reportNumber, string fileName, string referenceAnexo, bool isSubAnexo)
        {
            try
            {
                var dataSet = _db.GetMostRecentAnexoReference(clavePlaza, referenceAnexo, isSubAnexo);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("spMostRecentSubAnexo retorna tabla vacía");
                DataTable TableAnexo = dataSet.Tables[0];

                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{reportNumber}\Reportes Fotograficos Equipo Nuevo\{TableAnexo.Rows[0]["NewerReference"]}\Imgs\{fileName}";
                if (!System.IO.File.Exists(path))
                    return NotFound(path);
                System.IO.File.Delete(path);
                if (Directory.GetFiles($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{reportNumber}\Reportes Fotograficos Equipo Nuevo\{TableAnexo.Rows[0]["NewerReference"]}\Imgs").Length == 0)
                    Directory.Delete($@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{reportNumber}\Reportes Fotograficos Equipo Nuevo\{TableAnexo.Rows[0]["NewerReference"]}\Imgs");
                return Ok(path);
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: DeleteEquipoNuevoImg", 2);
                return NotFound(ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpGet("Nuevo/{clavePlaza}/{ubicacion}/{referenceNumber}/{referenceAnexo}/{isSubAnexo}")]
        public IActionResult GetReporteEquipoNuevo(string clavePlaza, string ubicacion, string referenceNumber, string referenceAnexo, bool isSubAnexo)
        {
            try
            {
                var dataSet = _db.GetStoreNuevoPDF(clavePlaza, referenceNumber, referenceAnexo, isSubAnexo);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("GetStoredPdf retorna tabla vacía");
                ReporteFotograficoPdfCreation pdf = new ReporteFotograficoPdfCreation(clavePlaza, dataSet.Tables[0], new ApiLogger(), 2, referenceNumber, ubicacion, referenceAnexo);
                var pdfResult = pdf.NewPdf($@"{this._disk}:\{this._folder}");
                if (pdfResult.Result == null)
                    return NotFound(pdfResult.Message);
                return File(new FileStream(pdfResult.Result.ToString(), FileMode.Open, FileAccess.Read), "application/pdf");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "ReporteFotograficoController: GetReporteEquipoDañado", 2);
                return NotFound(ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpPost("CopyAnexoImages/{clavePlaza}/{referenceNumber}/{referenceAnexo}")]
        public ActionResult<string> DeleteEquipoNuevoImgs(string clavePlaza, string referenceNumber, string referenceAnexo)
        {
            try
            {
                var dataSet = _db.GetMostRecentAnexoReference(clavePlaza, referenceAnexo, true);
                if (dataSet.Tables[0].Rows.Count == 0)
                    return NotFound("spMostRecentSubAnexo retorna tabla vacía");

                string path = $@"{this._disk}:\{this._folder}\{clavePlaza.ToUpper()}\DTC\{referenceNumber}\Reportes Fotograficos Equipo Nuevo";
                DataTable TableAnexo = dataSet.Tables[0];
                string subAnexoMasReciente = TableAnexo.Rows[0]["NewerReference"].ToString();

                if (subAnexoMasReciente.Equals(referenceAnexo + "-2"))
                {
                    CopyDirectory($@"{path}\{referenceAnexo}\Imgs", $@"{path}\{subAnexoMasReciente}\Imgs", false);
                    CopyDirectory($@"{path}\{referenceAnexo}\ImgsFullSize", $@"{path}\{subAnexoMasReciente}\ImgsFullSize", false);
                }
                else
                {
                    string carpetaPasada = subAnexoMasReciente.Substring(subAnexoMasReciente.Length - 2);
                    if (carpetaPasada.Contains('-'))
                        carpetaPasada = carpetaPasada.Replace("-", string.Empty);
                    int numCarpetaPasada = Int16.Parse(carpetaPasada) - 1;

                    CopyDirectory($@"{path}\{referenceAnexo}-{numCarpetaPasada}\Imgs", $@"{path}\{subAnexoMasReciente}\Imgs", false);
                    CopyDirectory($@"{path}\{referenceAnexo}-{numCarpetaPasada}\ImgsFullSize", $@"{path}\{subAnexoMasReciente}\ImgsFullSize", false);
                }
                return Ok("Los archivos se han copiado exitosamente a la carpeta: " + $@"{path}\{subAnexoMasReciente}\Imgs");
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(clavePlaza, ex, "AnexoDTCController: DeleteAnexo", 2);
                return NotFound(ex.ToString());
            }
        }
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}