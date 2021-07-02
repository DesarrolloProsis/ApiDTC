

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
        private readonly string _clavePlaza;

        private readonly ApiLogger _apiLogger;

        private readonly FichaTecnicaInfo _fichaTecnicaInfo;

        #endregion

         #region Pdf Configuration
        //Tipo de Letras 
        #region BaseFont
        public static BaseFont NegritaGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChicaSubAzul = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteLetrita = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteMini = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion
        #region iText.Font
        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 13f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNormalChicaFirmas = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMedianaRoja = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Red);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion

        #endregion

        #region Constructors


        public FichaTecnicaAtencionPdfCreation(string clavePlaza, ApiLogger apiLogger, FichaTecnicaInfo fichaTecnica)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _fichaTecnicaInfo = fichaTecnica;
        }

        #endregion
        //https://localhost:44358/api/ReporteFotografico/Reporte/JOR/B01
        #region Methods
        public Response NewPdf(string folder)
        {
            string directory, filename, path;

            DateTime now = DateTime.Now; 
            
            directory = $@"{folder}\{_clavePlaza.ToUpper()}\Reportes\{_fichaTecnicaInfo.NumeroReporte}";
            filename = $"{_fichaTecnicaInfo.NumeroReporte}-FichaTecnica.pdf";
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
                            Message = $"Error: Archivo {_fichaTecnicaInfo.NumeroReporte} en uso o inaccesible",
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
                    doc.Add(TablaInformacion());
                    doc.Add(TablaTipoFalla());
                    doc.Add(TablaObservaciones());

                    //PRUEBA IMÁGENES, CAMBIAR RUTA por la carpeta de DiagnosticoFallaImgs cuando  _fichaTecnicaInfo.TipoFalloId sea 2 o 3 codemcm
                    string directoryImgs = "";

                    if (_fichaTecnicaInfo.TipoFalloId == 2 || _fichaTecnicaInfo.TipoFalloId == 3)//DESCOMENTAR ESTE CAMBIO... CODEMCM
                    {
                        directoryImgs = Path.Combine(directory, "DiagnosticoFallaImgs");
                    }
                    else
                    {
                        directoryImgs = Path.Combine(directory, "FichaTecnicaAtencionImgs");
                    }
                    string[] fotos = new string[4];
                    if(Directory.Exists(directoryImgs))
                    {
                        fotos = Directory.GetFiles(directoryImgs);
                        doc.Add(TablaFotografias(fotos));//FIXME: Revisar agregue este metodo codemcm
                        foreach (var img in Directory.GetFiles(directoryImgs))
                        {
                            if(img.Contains("temp"))
                                File.Delete(img);
                        }
                    }
                    
                    PdfContentByte cb = writer.DirectContent;
                    PdfPTable tablaFirmas = TablaFirmas();
                    tablaFirmas.WriteSelectedRows(0, -1, 30, 170, cb);
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
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 20f, 20f, 20f, 20f, 20f }) { WidthPercentage = 100f };

                string textoTitulo = "FICHA TÉCNICA DE ATENCIÓN";
                var colTitulo = new PdfPCell(new Phrase(textoTitulo, letraoNegritaGrande)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 3 };
                CeldasVacias(1, table);
                table.AddCell(colTitulo);
                CeldasVacias(1, table);

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

        private IElement TablaFotografias(string[] rutas)
        {

            try
            {
                PdfPTable table = new PdfPTable(new float[] {50f, 50f }) { WidthPercentage = 100f };
                
                var colEvidencia = new PdfPCell(new Phrase("EVIDENCIA FOTOGRÁFICA DE LA FALLA REPORTADA:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                CeldasVacias(2, table);
                table.AddCell(colEvidencia);   
                CeldasVacias(1, table);

                foreach (var foto in rutas)
                {
                    //Si procesa un archivo temporal que no se eliminó
                    if(foto.Contains("-temp"))
                    {
                        File.Delete(foto);
                        continue;
                    }
                    System.Drawing.Image imageReview = System.Drawing.Image.FromFile(foto);
                    string fotoTemporal = foto.Substring(0, foto.LastIndexOf('.')) + "-temp.jpg";
                    foreach (var prop in imageReview.PropertyItems)
                    {
                        if(prop.Id == 0x0112)
                        {
                            int orientationValue = imageReview.GetPropertyItem(prop.Id).Value[0];
                            System.Drawing.RotateFlipType rotateFlipType = GetOrientationToFlipType(orientationValue);
                            imageReview.RotateFlip(rotateFlipType);
                            imageReview.RemovePropertyItem(0x0112);
                            if(!File.Exists(fotoTemporal))
                                File.Delete(fotoTemporal);
                            imageReview.Save(fotoTemporal);
                            break;
                        }
                    }
                    if(!File.Exists(fotoTemporal))
                        imageReview.Save(fotoTemporal);
                    Image img = Image.GetInstance(fotoTemporal);
                    if (img.Width > img.Height)
                        img.ScaleAbsolute(110f, 90f);
                    else
                        img.ScaleAbsolute(90f, 110f);
                    PdfPCell colFoto = new PdfPCell(img) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                    table.AddCell(colFoto);
                }

                for (int i = 0; i < 4 - rutas.Length; i++)
                {
                    Image logo = Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\sinImagen.png");
                    logo.ScaleAbsolute(90f, 110f);
                    PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                    table.AddCell(colLogo);
                }

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "DiagnosticoFallaPdfCreation: TablaFotografias", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "DiagnosticoFallaPdfCreation: TablaFotografias", 3);
                return null;
            }
        }

        private IElement TablaInformacion()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                CeldasVacias(8, table);
                var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };

                var colNoReporte = new PdfPCell(new Phrase(_fichaTecnicaInfo.NumeroReporte, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                var colTextoFecha = new PdfPCell(new Phrase("Fecha:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };
                var colFecha = new PdfPCell(new Phrase(Convert.ToString(_fichaTecnicaInfo.Fecha).Substring(0,10), letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colTextoNoReporte);
                table.AddCell(colNoReporte);
                CeldasVacias(1, table);
                table.AddCell(colTextoFecha);
                table.AddCell(colFecha);
                CeldasVacias(1, table);

                //Plaza de cobro

                var colPlazaDeCobro = new PdfPCell(new Phrase("Plaza de Cobro:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };

                var plazaDeCobro = new PdfPCell(new Phrase(_fichaTecnicaInfo.PlazaCobro, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                var colTextoHoraInicio = new PdfPCell(new Phrase("Hora INICIO:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };
                var colHoraInicio = new PdfPCell(new Phrase(Convert.ToString(_fichaTecnicaInfo.Inicio).Substring(0, 10), letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colPlazaDeCobro);
                table.AddCell(plazaDeCobro);
                CeldasVacias(1, table);
                table.AddCell(colTextoHoraInicio);
                table.AddCell(colHoraInicio);
                CeldasVacias(1, table);

                //Ubicación

                var colUbicacion = new PdfPCell(new Phrase("Ubicación:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };

                var ubicacion = new PdfPCell(new Phrase(_fichaTecnicaInfo.Ubicacion, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                var colTextoHoraFin = new PdfPCell(new Phrase("Hora FIN:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };
                var colHoraFin = new PdfPCell(new Phrase(Convert.ToString(_fichaTecnicaInfo.Fin).Substring(0, 10), letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colUbicacion);
                table.AddCell(ubicacion);
                CeldasVacias(1, table);
                table.AddCell(colTextoHoraFin);
                table.AddCell(colHoraFin);
                CeldasVacias(1, table);

                //Folio falla
                var colFolioFalla = new PdfPCell(new Phrase("Folio de FALLA:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, Colspan = 3 };

                var falla = new PdfPCell(new Phrase(_fichaTecnicaInfo.FolioFalla, letraNormalMedianaRoja)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 4 };


                table.AddCell(colFolioFalla);
                table.AddCell(falla);
                CeldasVacias(1, table);

                var colNoSiniestro = new PdfPCell(new Phrase("Folio de SINIESTRO:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, Colspan = 3 };

                var siniestro = new PdfPCell(new Phrase(_fichaTecnicaInfo.NumeroSiniestro, letraNormalMedianaRoja)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 4 };


                table.AddCell(colNoSiniestro);
                table.AddCell(siniestro);
                CeldasVacias(1, table);

                //Técnico
                var colTecnico = new PdfPCell(new Phrase("Técnico Responsable PROSIS:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, Colspan = 3 };

                var tecnico = new PdfPCell(new Phrase(_fichaTecnicaInfo.TecnicoProsis, letraNormalMedianaRoja)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 4 };


                table.AddCell(colTecnico);
                table.AddCell(tecnico);
                CeldasVacias(1, table);

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

        private IElement TablaTipoFalla()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                CeldasVacias(16, table);
                var colTipoFalla = new PdfPCell(new Phrase("TIPO DE FALLA:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, Rowspan = 2 };

                var colCuadroOperacion = new PdfPCell(new Phrase((_fichaTecnicaInfo.TipoFalloId == 1) ? "X" : " ", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Rowspan = 2 };
                var colTextoOperacion = new PdfPCell(new Phrase("POR OPERACIÓN", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, Rowspan = 2 };
                //2 POR SINIESTRO
                var colCuadroSiniestro = new PdfPCell(new Phrase((_fichaTecnicaInfo.TipoFalloId == 2) ? "X" : " ", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Rowspan = 2 };
                var colTextoSiniestro = new PdfPCell(new Phrase("POR SINIESTRO", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, Rowspan = 2 };
                //3 VIDA UTIL
                var colCuadroVidaUtil = new PdfPCell(new Phrase((_fichaTecnicaInfo.TipoFalloId == 3) ? "X" : " ", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Rowspan = 2 };
                var colTextoVidaUtil = new PdfPCell(new Phrase("POR FIN DE VIDA UTIL", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, Rowspan = 2 };


                table.AddCell(colTipoFalla);
                CeldasVacias(1, table);
                table.AddCell(colCuadroOperacion);
                table.AddCell(colTextoOperacion);
                table.AddCell(colCuadroSiniestro);
                table.AddCell(colTextoSiniestro);
                table.AddCell(colCuadroVidaUtil);
                table.AddCell(colTextoVidaUtil);

                CeldasVacias(8, table);

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
                CeldasVacias(16, table);


                var colDescripcion = new PdfPCell(new Phrase("DESCRIPCIÓN DE LA FALLA REPORTADA:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 6 };

                table.AddCell(colDescripcion);
                CeldasVacias(2, table);

                //Descripción de falla
                var textoDescripcion = SeparacionObservaciones(_fichaTecnicaInfo.DescripcionFalla);
                int celdasTotalesDescripcion = 0;
                foreach (var linea in textoDescripcion)
                {
                    celdasTotalesDescripcion +=1;
                    var celdaLinea = new PdfPCell(new Phrase(Convert.ToString(linea), letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                for (int i = 0; i < 3 - celdasTotalesDescripcion; i++)
                {
                    var celdaLinea = new PdfPCell(new Phrase("", letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                CeldasVacias(16, table);

                //Diagnóstico de falla
                var colDiagnostico = new PdfPCell(new Phrase("SOLUCIÓN y/o INTERVENCIÓN REALIZADA PARA LA FALLA REPORTADA:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 6 };

                table.AddCell(colDiagnostico);
                CeldasVacias(2, table);
                var textoDiagnostico = SeparacionObservaciones(_fichaTecnicaInfo.Intervencion);
                int celdasTotalesDiagnostico = 0;
                foreach (var linea in textoDiagnostico)
                {
                    celdasTotalesDiagnostico +=1;
                    var celdaLinea = new PdfPCell(new Phrase(Convert.ToString(linea), letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                for (int i = 0; i < 3 - celdasTotalesDiagnostico; i++)
                {
                    var celdaLinea = new PdfPCell(new Phrase("", letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                CeldasVacias(16, table);

                

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

        private PdfPTable TablaFirmas()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 30f, 10f, 30f, 10f, 30f }) { WidthPercentage = 100f };
                table.TotalWidth = 550;
                CeldasVacias(15, table);
                var celdaVaciaFirmas = new PdfPCell() { Border = 0, FixedHeight = 30 };
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() {BorderWidthTop = 1, BorderWidthBottom = 0, BorderWidthLeft = 1, BorderWidthRight = 1, FixedHeight = 30 });
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() {BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 1, BorderWidthRight = 1, FixedHeight = 30 });
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() {BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 1, BorderWidthRight = 1, FixedHeight = 30 });
                //NOMRE Y FIRMA
                var colNombre = new PdfPCell(new Phrase("Nombre y Firma", letraoNormalChicaFirmas))
                {
                    BorderWidth = 0,
                    BorderWidthTop = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 2
                };

                var colSello = new PdfPCell(new Phrase("Sello de Plaza de Cobro", letraoNormalChicaFirmas))
                {
                    BorderWidth = 0,
                    BorderWidthTop = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 2
                };

                table.AddCell(colNombre);
                CeldasVacias(1, table);
                table.AddCell(colNombre);
                CeldasVacias(1, table);
                table.AddCell(colSello);

                //Técnico
                var colTecnico = new PdfPCell(new Phrase(_fichaTecnicaInfo.TecnicoProsis, letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var colPersonal = new PdfPCell(new Phrase(_fichaTecnicaInfo.AdministradorPlaza, letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                table.AddCell(colTecnico);
                CeldasVacias(1, table);
                table.AddCell(colPersonal);
                CeldasVacias(2, table);

                //
                var colProsis = new PdfPCell(new Phrase("Proyectos y Sistemas Informáticos S.A. de C.V.", letraNormalChica))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var colCapufe= new PdfPCell(new Phrase("CAPUFE", letraoNormalChicaFirmas))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                table.AddCell(colProsis);
                CeldasVacias(1, table);
                table.AddCell(colCapufe);
                CeldasVacias(2, table);
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

        public void CeldasVacias(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell() { Border = 0 });
        }

        private List<string> SeparacionObservaciones(string observaciones)
        {
            List<string> lineaObservaciones = new List<string>();
            if(observaciones.Length <= 100)
            {
                lineaObservaciones.Add(observaciones);
                return lineaObservaciones;
            }

            string linea = string.Empty;
            for (int i = 0; i < observaciones.Length; i++)
            {
                if(observaciones[i].Equals(',') || observaciones[i].Equals('.') || observaciones[i].Equals('.') || observaciones[i].Equals(':'))
                {
                    if(i < observaciones.Length - 1 && !observaciones[i + 1].Equals(' '))
                    {
                        linea += $"{observaciones[i]} ";
                        continue;
                    }
                }
                linea += observaciones[i];
                if(linea.Length >= 100)
                {
                    lineaObservaciones.Add(linea);
                    linea = string.Empty;
                }
            }
            lineaObservaciones.Add(linea);

            /*char[] separadores = new char[]{
                ' ',
                ',',
                '.'
            };
            var palabras = observaciones.Split(separadores);
            List<string> palabrasSinVacio = new List<string>();
            foreach(var palabra in palabras)
            {
                if(!string.IsNullOrEmpty(palabra))
                    palabrasSinVacio.Add(palabra);
            }
            string linea = string.Empty;
            foreach (var palabra in palabrasSinVacio)
            {
                if(!string.IsNullOrEmpty(palabra))
                {
                    linea += $"{palabra} ";
                    if(linea.Length > 100)
                    {
                        lineaObservaciones.Add(linea);
                        linea = string.Empty;
                    }
                    if(palabra == palabrasSinVacio[palabrasSinVacio.Count - 1] && linea.Length < 100)
                    {
                        lineaObservaciones.Add(linea);
                        linea = string.Empty;
                    }
                }
            }*/
            return lineaObservaciones;
        }
        private string MesActual() { return new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(DateTime.Now.Month); }
 
        private static System.Drawing.RotateFlipType GetOrientationToFlipType(int orientationValue)
        {
            System.Drawing.RotateFlipType rotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone;

            switch (orientationValue)
            {
                case 1:
                    rotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    rotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    rotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    rotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    rotateFlipType = System.Drawing.RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    rotateFlipType = System.Drawing.RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    rotateFlipType = System.Drawing.RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    rotateFlipType = System.Drawing.RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    rotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone;
                    break;
            }

            return rotateFlipType;
        }
        #endregion
    }
}
