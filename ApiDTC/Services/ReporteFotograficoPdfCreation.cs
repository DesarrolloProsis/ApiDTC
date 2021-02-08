

namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    public class ReporteFotograficoPdfCreation
    {
        #region Attributes

        private readonly DataTable _tableHeader;

        private readonly string _clavePlaza;

        private readonly ApiLogger _apiLogger;

        private readonly int _tipo;

        private readonly string _ubicacion;

        private readonly string _referenceNumber;
        #endregion

        #region Pdf Configuration
        //Tipo de Letras 
        #region BaseFont
        public static BaseFont fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChicaSubAzul = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteLetrita = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteMini = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalMedianaInline = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion
        #region iText.Font
        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 13f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 7f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalGrande = new iTextSharp.text.Font(NormalGrande, 15f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraSubAzulChica = new iTextSharp.text.Font(NormalChicaSubAzul, 5f, iTextSharp.text.Font.UNDERLINE, BaseColor.Blue);
        public static iTextSharp.text.Font letritasMiniMini = new iTextSharp.text.Font(fuenteLetrita, 1f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion
        #region Logo
        #endregion
        #endregion

        #region Constructors

        public ReporteFotograficoPdfCreation(string clavePlaza, DataTable tableHeader, ApiLogger apiLogger, int tipo, string referenceNumber, string ubicacion)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _tipo = tipo;
            _referenceNumber = referenceNumber;
            _ubicacion = ubicacion;
        }

        public ReporteFotograficoPdfCreation(string clavePlaza, ApiLogger apiLogger, int tipo, string ubicacion, string referenceNumber)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tipo = tipo;
            _ubicacion = ubicacion;
            _referenceNumber = referenceNumber;
        }

        #endregion
        //https://localhost:44358/api/ReporteFotografico/Reporte/JOR/B01
        #region Methods
        public Response NewPdf()
        {
            string directory, filename, path;

            DateTime now = DateTime.Now; 
            if (_tipo == 1)
                directory = $@"C:\Bitacora\{_clavePlaza.ToUpper()}\Reportes\{_referenceNumber}";
            else
                directory = $@"C:\Bitacora\{_clavePlaza.ToUpper()}\DTC\{_referenceNumber}";
            if (!Directory.Exists(directory))
                return new Response
                {
                    Message = "Error: No existe el directorio",
                    Result = null
                };

            if (_tipo == 1)
                filename = $"ReporteFotográfico-{_referenceNumber}.pdf";
            else if(_tipo == 2)
                filename = $@"DTC-{_referenceNumber}-EquipoNuevo.pdf";
            else
                filename = $@"DTC-{_referenceNumber}-EquipoDañado.pdf";

            path = Path.Combine(directory, filename);
            
            //File in use
            try
            {   
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                if (File.Exists(path))
                {
                    if (FileInUse(path))
                    {
                        return new Response
                        {
                            Message = $"Error: Archivo {filename}.pdf en uso o inaccesible",
                            Result = null
                        };
                    }
                }
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: NewPdf", 2);
                return new Response
                {
                    Message = $"Error: {ex.Message}.",
                    Result = null
                };
            }

            Document doc = new Document();
            try
            {
                using(MemoryStream myMemoryStream = new MemoryStream())
                {
                    doc.SetPageSize(new Rectangle(609.4488f, 793.701f));
                    doc.SetMargins(35f, 35f, 30f, 30f);
                    doc.AddAuthor("PROSIS");
                    switch (_tipo)
                    {
                        case 1: 
                            doc.AddTitle("REPORTE FOTOGRÁFICO MANTENIMIENTO PREVENTIVO");  
                            break;
                        case 2:
                            doc.AddTitle("REPORTE FOTOGRÁFICO MANTENIMIENTO CORRECTIVO EQUIPO NUEVO");
                            break;
                        case 3:
                            doc.AddTitle("REPORTE FOTOGRÁFICO MANTENIMIENTO CORRECTIVO EQUIPO DAÑADO");
                            break;
                        default: break;
                    }
                    

                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelperVertical();
                    writer.Open();

                    doc.Open();
                    

                    doc.Add(TablaEncabezado());
                    doc.Add(new Phrase(" "));
                    doc.Add(TablaInformacion());
                    string directoryImgs;
                    if (_tipo == 1)
                        directoryImgs = Path.Combine(directory, "Imgs");
                    else if (_tipo == 2)
                        directoryImgs = Path.Combine(directory, "EquipoNuevoImgs");
                    else
                        directoryImgs = Path.Combine(directory, "EquipoDañadoImgs");
                    var fotos = Directory.GetFiles(directoryImgs);
                    if (fotos.Length <= 4 && fotos.Length > 0)
                        doc.Add(TablaFotografias(fotos, 4));
                    else if(fotos.Length > 4 && fotos.Length <= 6)
                        doc.Add(TablaFotografias(fotos, 6));

                    doc.Add(TablaObservaciones());
                    doc.Add(TablaFirmas());
                    doc.Close();
                    writer.Close();
                    byte[] content = myMemoryStream.ToArray();


                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(content, 0, (int)content.Length);
                    }
                }
            }
            catch (IOException ex)
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: NewPdf", 2);
                return new Response
                {
                    Message = $"Error: {ex.Message}.",
                    Result = null
                };
            }
            return new Response
            {
                Message = "Ok",
                Result = path
            };
        }

        private IElement TablaEncabezado()
        {
            try
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\prosis-logo.jpg");
                logo.ScalePercent(10f);

                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 20f, 20f, 20f, 20f, 20f }) { WidthPercentage = 100f };

                var celdaVacia = new PdfPCell() { Border = 0 };
                PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                table.AddCell(colLogo);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                var celdaSalto = new PdfPCell() { Colspan = 5, Border = 0 };
                table.AddCell(celdaSalto);
                string textoTitulo = "";
                switch (_tipo)
                {
                    case 1:
                        textoTitulo = "REPORTE FOTOGRÁFICO MANTENIMIENTO PREVENTIVO";
                        break;
                    case 2:
                        textoTitulo = "REPORTE FOTOGRÁFICO MANTENIMIENTO CORRECTIVO EQUIPO NUEVO";
                        break;
                    case 3:
                        textoTitulo = "REPORTE FOTOGRÁFICO MANTENIMIENTO CORRECTIVO EQUIPO DAÑADO";
                        break;
                    default: break;
                }
                var colTitulo = new PdfPCell(new Phrase(textoTitulo, letraNormalMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingRight = 10, PaddingLeft = 10, Colspan = 3 };
                table.AddCell(celdaVacia);
                table.AddCell(colTitulo);
                table.AddCell(celdaVacia);

                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch(Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaEncabezado", 3);
                return null;
            }
            
        }

        private IElement TablaFotografias(string[] rutas, int columnas)
        {

            try
            {
                PdfPTable table;
                if(columnas == 4)
                    table = new PdfPTable(new float[] {50f, 50f }) { WidthPercentage = 100f };
                else
                    table = new PdfPTable(new float[] { 33.33f, 33.33f, 33.33f }) { WidthPercentage = 80f };

                var celdaVacia = new PdfPCell() { Border = 0, FixedHeight = 10 };

                if(columnas == 4)
                {
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                }
                else
                {
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                }          

                foreach (var foto in rutas)
                {
                    Image img = Image.GetInstance(foto);
                    if(columnas == 4)
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(190f, 150f);
                        else
                            img.ScaleAbsolute(150f, 190f);
                    }
                    else
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(130f, 140f);
                        else
                            img.ScaleAbsolute(140f, 130f);
                    }
                    PdfPCell colFoto = new PdfPCell(img) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                    table.AddCell(colFoto);
                }

                for (int i = 0; i < columnas - rutas.Length; i++)
                    table.AddCell(celdaVacia);

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaFotografias", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaFotografias", 3);
                return null;
            }
        }

        private IElement TablaInformacion()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                var celdaVacia = new PdfPCell() { Border = 0 };

                var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };

                string valorReporte = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["NumeroReporte"]) : Convert.ToString(_tableHeader.Rows[0]["Referencia"]);
                var colNoReporte = new PdfPCell(new Phrase(valorReporte, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };

                string valorFecha = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Fecha"]).Substring(0, 8) : Convert.ToString(_tableHeader.Rows[0]["Fecha"]);
                var colTextoFecha = new PdfPCell(new Phrase("Fecha: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
                var colFecha = new PdfPCell(new Phrase(valorFecha, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colTextoNoReporte);
                table.AddCell(colNoReporte);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoFecha);
                table.AddCell(colFecha);
                table.AddCell(celdaVacia);

                //Plaza de cobro

                var colPlazaDeCobro = new PdfPCell(new Phrase("Plaza de Cobro: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };

                string valorPlaza = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Plaza"]) : Convert.ToString(_tableHeader.Rows[0]["Plaza"]);
                var plazaDeCobro = new PdfPCell(new Phrase(valorPlaza, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };

                string valorHoraInicio = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Inicio"]) : Convert.ToString(_tableHeader.Rows[0]["HoraInicio"]);
                var colTextoHoraInicio = new PdfPCell(new Phrase("Hora INICIO: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
                var colHoraInicio = new PdfPCell(new Phrase(valorHoraInicio, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colPlazaDeCobro);
                table.AddCell(plazaDeCobro);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoHoraInicio);
                table.AddCell(colHoraInicio);
                table.AddCell(celdaVacia);

                //Ubicación

                //string valorUbicacion= _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Ubicacion"]) : Convert.ToString(_tableHeader.Rows[0]["Ubicacion"]);
                string valorUbicacion = _ubicacion;
                var colUbicacion = new PdfPCell(new Phrase("Ubicación: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };

                var ubicacion = new PdfPCell(new Phrase(valorUbicacion, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };

                string valorHoraFin = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Fin"]) : Convert.ToString(_tableHeader.Rows[0]["HoraFin"]);
                var colTextoHoraFin = new PdfPCell(new Phrase("Hora FIN: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
                var colHoraFin = new PdfPCell(new Phrase(valorHoraFin, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colUbicacion);
                table.AddCell(ubicacion);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoHoraFin);
                table.AddCell(colHoraFin);
                table.AddCell(celdaVacia);

                //Técnico
                var colTecnico = new PdfPCell(new Phrase("Técnico Responsable PROSIS: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 3 };
                string valorTecnicoProsis = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["TecnicoProsis"]) : Convert.ToString(_tableHeader.Rows[0]["Tecnico"]);
                var tecnico = new PdfPCell(new Phrase(valorTecnicoProsis, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                table.AddCell(colTecnico);
                table.AddCell(tecnico);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);


                //Personal CAPUFE

                string valorCapufe = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["PersonalCapufe"]) : Convert.ToString(_tableHeader.Rows[0]["PersonalCapufe"]);
                var colCapufe= new PdfPCell(new Phrase("Personal de Plaza de Cobro CAPUFE: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 3 };
                var capufe = new PdfPCell(new Phrase(valorCapufe, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                table.AddCell(colCapufe);
                table.AddCell(capufe);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                //Equipo nuevo o dañado
                if (_tipo != 1)
                {
                    var colSiniestro = new PdfPCell(new Phrase("No. de Siniestro: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 3 };

                    var siniestro = new PdfPCell(new Phrase(Convert.ToString(_tableHeader.Rows[0]["NumeroSiniestro"]), letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                    table.AddCell(colSiniestro);
                    table.AddCell(siniestro);
                    table.AddCell(celdaVacia);
                    table.AddCell(celdaVacia);
                }

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaInformacion", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaInformacion", 3);
                return null;
            }
        }

        private IElement TablaObservaciones()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                var celdaVacia = new PdfPCell() { Border = 0 };

                //ESPACIO
                for (int i = 0; i < 24; i++)
                {
                    table.AddCell(celdaVacia);
                }


                var colTextoObservaciones = new PdfPCell(new Phrase("Observaciones: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 2 };

                table.AddCell(colTextoObservaciones);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                var celdaObservaciones = new PdfPCell(new Phrase(Convert.ToString(_tableHeader.Rows[0]["Observation"]), letraNormalChica)) { BorderWidth = 0, VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_JUSTIFIED, Colspan = 8 };

                table.AddCell(celdaObservaciones);

                for (int i = 0; i < 16; i++)
                {
                    table.AddCell(celdaVacia);
                }

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaObservaciones", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaObservaciones", 3);
                return null;
            }   
        }

        private IElement TablaFirmas()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 30f, 10f, 30f, 10f, 30f }) { WidthPercentage = 100f };

                var celdaVacia = new PdfPCell() { Border = 0 };
                
                var celdaVaciaFirmas = new PdfPCell() { Border = 0, FixedHeight = 30 };
                for (int i = 0; i < 15; i++)
                    table.AddCell(celdaVaciaFirmas);

                //NOMRE Y FIRMA
                var colNombre = new PdfPCell(new Phrase("Nombre y Firma", letraoNegritaChica))
                {
                    BorderWidth = 0,
                    BorderWidthTop = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 2
                };

                var colSello = new PdfPCell(new Phrase("Sello de Plaza de Cobro", letraoNegritaChica))
                {
                    BorderWidth = 0,
                    BorderWidthTop = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 2
                };

                table.AddCell(colNombre);
                table.AddCell(celdaVacia);
                table.AddCell(colNombre);
                table.AddCell(celdaVacia);
                table.AddCell(colSello);

                //Técnico
                string valorTecnicoProsis = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["TecnicoProsis"]) : Convert.ToString(_tableHeader.Rows[0]["Tecnico"]);
                var colTecnico = new PdfPCell(new Phrase(valorTecnicoProsis, letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                string valorPersonalCapufe = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["PersonalCapufe"]) : Convert.ToString(_tableHeader.Rows[0]["PersonalCapufe"]);
                var colPersonal = new PdfPCell(new Phrase(valorPersonalCapufe, letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                table.AddCell(colTecnico);
                table.AddCell(celdaVacia);
                table.AddCell(colPersonal);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                //
                var colProsis = new PdfPCell(new Phrase("Proyectos y Sistemas Informáticos S.A. de C.V.", letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var colCapufe= new PdfPCell(new Phrase("Personal Plaza de Cobro", letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                table.AddCell(colProsis);
                table.AddCell(celdaVacia);
                table.AddCell(colCapufe);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaFirmas", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaFirmas", 3);
                return null;
            }
        }

        private bool FileInUse(string file)
        {
            bool fileInUse = false;
            try
            {
                FileStream fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: FileInUse", 5);
                return true;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: FileInUse", 3);
                return true;
            }
            return fileInUse;
        }
        #endregion
    }
}
