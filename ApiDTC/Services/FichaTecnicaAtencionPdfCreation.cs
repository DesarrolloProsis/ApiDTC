

namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    public class FichaTecnicaAtencionPdfCreation
    {
        #region Attributes

        private readonly DataTable _tableHeader;

        private readonly string _clavePlaza;

        private readonly ApiLogger _apiLogger;

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
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalGrande = new iTextSharp.text.Font(NormalGrande, 15f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraSubAzulChica = new iTextSharp.text.Font(NormalChicaSubAzul, 5f, iTextSharp.text.Font.UNDERLINE, BaseColor.Blue);
        public static iTextSharp.text.Font letritasMiniMini = new iTextSharp.text.Font(fuenteLetrita, 1f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion
        #region Logo
        #endregion
        #endregion

        #region Constructors

        public FichaTecnicaAtencionPdfCreation(string clavePlaza, DataTable tableHeader, ApiLogger apiLogger, string ubicacion, string referenceNumber)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _ubicacion = ubicacion;
            _referenceNumber = referenceNumber;
        }

        public FichaTecnicaAtencionPdfCreation(string clavePlaza, ApiLogger apiLogger, string ubicacion, string referenceNumber)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
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
            
            directory = $@"C:\Bitacora\{_clavePlaza.ToUpper()}\Reportes";
            filename = $"FichaTecnicaAtencion-{_referenceNumber}";
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
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: NewPdf", 2);
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
                    doc.AddTitle("FICHA TÉCNICA DE ATENCIÓN");  
                    
                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelperVertical();
                    writer.Open();

                    doc.Open();
                    

                    doc.Add(TablaEncabezado());
                    doc.Add(new Phrase(" "));
                    doc.Add(TablaInformacion());
                    doc.Add(TablaObservaciones());

                    //PRUEBA IMÁGENES, CAMBIAR RUTA
                    string directoryImgs = $@"{System.Environment.CurrentDirectory}\Reportes\2020\septiembre\24\Prueba\";
                    var fotos = Directory.GetFiles(directoryImgs);
                    if (fotos.Length <= 4 && fotos.Length > 0)
                        doc.Add(TablaFotografias(fotos, 4));
                    else if (fotos.Length > 4 && fotos.Length <= 6)
                        doc.Add(TablaFotografias(fotos, 6));
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
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: NewPdf", 2);
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
                string textoTitulo = "FICHA TÉCNICA DE ATENCIÓN";
                var colTitulo = new PdfPCell(new Phrase(textoTitulo, letraNormalGrande)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingRight = 10, PaddingLeft = 10, Colspan = 3 };
                table.AddCell(celdaVacia);
                table.AddCell(colTitulo);
                table.AddCell(celdaVacia);

                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch(Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaEncabezado", 3);
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
                            img.ScaleAbsolute(140f, 130f);
                        else
                            img.ScaleAbsolute(130f, 140f);
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
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaFotografias", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaFotografias", 3);
                return null;
            }
        }

        private IElement TablaInformacion()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                var celdaVacia = new PdfPCell() { Border = 0 };

                var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                string valorReporte = "TLP-MPS-01";
                var colNoReporte = new PdfPCell(new Phrase(valorReporte, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };
                
                var colTextoFecha = new PdfPCell(new Phrase("Fecha: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
                var colFecha = new PdfPCell(new Phrase("15/07/2020", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colTextoNoReporte);
                table.AddCell(colNoReporte);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoFecha);
                table.AddCell(colFecha);
                table.AddCell(celdaVacia);

                //Plaza de cobro

                var colPlazaDeCobro = new PdfPCell(new Phrase("Plaza de Cobro: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                var plazaDeCobro = new PdfPCell(new Phrase("001 TLALPAN", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                var colTextoHoraInicio = new PdfPCell(new Phrase("Hora INICIO: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
                var colHoraInicio = new PdfPCell(new Phrase("12:00:00 a.m.", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colPlazaDeCobro);
                table.AddCell(plazaDeCobro);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoHoraInicio);
                table.AddCell(colHoraInicio);
                table.AddCell(celdaVacia);

                //Ubicación

                var colUbicacion = new PdfPCell(new Phrase("Ubicación: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                var ubicacion = new PdfPCell(new Phrase(_ubicacion, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                var colTextoHoraFin = new PdfPCell(new Phrase("Hora FIN: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
                var colHoraFin = new PdfPCell(new Phrase("12:00:00 a.m.", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colUbicacion);
                table.AddCell(ubicacion);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoHoraFin);
                table.AddCell(colHoraFin);
                table.AddCell(celdaVacia);

                //Folio falla
                var colFolioFalla = new PdfPCell(new Phrase("FOLIO DE FALLA:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                var falla = new PdfPCell(new Phrase("Número de Folio Módulo Institucional CAPUFE", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 4 };


                table.AddCell(colFolioFalla);
                table.AddCell(falla);
                table.AddCell(celdaVacia);

                //Técnico
                var colTecnico = new PdfPCell(new Phrase("Técnico Responsable PROSIS:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                var tecnico = new PdfPCell(new Phrase("NOMBRE DEL TÉCNICO", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 4 };


                table.AddCell(colTecnico);
                table.AddCell(tecnico);
                table.AddCell(celdaVacia);

                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                //Tipo falla

                var colTipoFalla = new PdfPCell(new Phrase("TIPO DE FALLA: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                var operacion = new PdfPCell(new Phrase("   ", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                var colPorOperacion = new PdfPCell(new Phrase("POR OPERACIÓN", letraNormalMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };
                
                var siniestro = new PdfPCell(new Phrase("   ", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                var colPorSiniestro = new PdfPCell(new Phrase("POR SINIESTRO", letraNormalMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };


                table.AddCell(colTipoFalla);
                table.AddCell(celdaVacia);
                table.AddCell(operacion);
                table.AddCell(colPorOperacion);
                table.AddCell(siniestro);
                table.AddCell(colPorSiniestro);
                

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaInformacion", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaInformacion", 3);
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


                var colFallaReportada = new PdfPCell(new Phrase("DESCRIPCIÓN DE LA FALLA REPORTADA: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 6 };

                table.AddCell(colFallaReportada);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);


                Chunk chunkFalla = new Chunk("What is Lorem Ipsum ? Lorem Ipsum is simply dummy text of the printing and typesetting industry.Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", letraNormalMediana).SetUnderline(0.5f, -1);
                var fallaReportada = new PdfPCell(new Phrase(chunkFalla)) { Border = 0, BorderWidthRight = 0, VerticalAlignment = Element.ALIGN_BOTTOM, HorizontalAlignment = Element.ALIGN_JUSTIFIED, Colspan = 8, Padding = 3 };

                table.AddCell(fallaReportada);
                
                for (int i = 0; i < 8; i++)
                {
                    table.AddCell(celdaVacia);
                }

                var colSolucion= new PdfPCell(new Phrase("SOLUCIÓN y/o INTERVENCIÓN REALIZADA PARA LA FALLA REPORTADA: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 6 };

                table.AddCell(colSolucion);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                Chunk chunkSolucion = new Chunk("What is Lorem Ipsum ? Lorem Ipsum is simply dummy text of the printing and typesetting industry.Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", letraNormalMediana).SetUnderline(0.5f, -1);
                var solucion = new PdfPCell(new Phrase(chunkSolucion)) { Border = 0, VerticalAlignment = Element.ALIGN_BOTTOM, HorizontalAlignment = Element.ALIGN_JUSTIFIED, Colspan = 8, Padding = 3 };

                table.AddCell(solucion);

                for (int i = 0; i < 8; i++)
                {
                    table.AddCell(celdaVacia);
                }

                var colEvidencia= new PdfPCell(new Phrase("EVIDENCIA FOTOGRÁFICA DE LA ATENCIÓN A LA FALLA REPORTADA: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 6 };

                table.AddCell(colEvidencia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                for (int i = 0; i < 16; i++)
                {
                    table.AddCell(celdaVacia);
                }

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaObservaciones", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaObservaciones", 3);
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
                for (int i = 0; i < 10; i++)
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
                var colTecnico = new PdfPCell(new Phrase("Técnico de Servicio", letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var colPersonal = new PdfPCell(new Phrase("Personal Plaza de Cobro", letraNormalChica))
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

                var colCapufe= new PdfPCell(new Phrase("CAPUFE", letraNormalChica))
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
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaFirmas", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: TablaFirmas", 3);
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
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: FileInUse", 5);
                return true;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "FichaTecnicaAtencionPdfCreation: FileInUse", 3);
                return true;
            }
            return fileInUse;
        }

        private string MesActual() { return new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(DateTime.Now.Month); }
 
        #endregion
    }
}
