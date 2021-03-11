namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
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
        public static BaseFont NegritaGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion
        #region iText.Font
        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 11f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 9f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNormalChicaFirmas = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
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

        #endregion
        //https://localhost:44358/api/ReporteFotografico/Reporte/JOR/B01
        #region Methods

        public Response NewPdf(string folder)
        {
            string directory, filename, path;
            if (_tipo == 1)
                directory = $@"{folder}\{_clavePlaza.ToUpper()}\Reportes\{_referenceNumber}";
            else
                directory = $@"{folder}\{_clavePlaza.ToUpper()}\DTC\{_referenceNumber}";
            if (!Directory.Exists(directory))
                return new Response
                {
                    Message = "Error: No existe el directorio",
                    Result = null
                };

            if (_tipo == 1)
                filename = $"ReporteFotográfico-{_referenceNumber}.pdf";
            else if (_tipo == 2)
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
                using (MemoryStream myMemoryStream = new MemoryStream())
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
                    doc.Add(TablaInformacion());
                    string directoryImgs;
                    if (_tipo == 1)
                        directoryImgs = Path.Combine(directory, "Imgs");
                    else if (_tipo == 2)
                        directoryImgs = Path.Combine(directory, "EquipoNuevoImgs");
                    else
                        directoryImgs = Path.Combine(directory, "EquipoDañadoImgs");
                    var files = Directory.GetFiles(directoryImgs);
                    List<string> fotos = new List<string>();
                    foreach (var file in files)
                        fotos.Add(file);
                    
                    if (fotos.Count <= 12)
                        doc.Add(TablaFotografias(fotos));
                    else
                    {
                        List<string> primerasFotos = new List<string>();
                        List<string> restoFotos = new List<string>();
                        for (int i = 0; i < 20; i++)
                        {
                            if(fotos.Count - 1 < i)
                                break;
                            primerasFotos.Add(fotos[i]);
                        }
                        if(fotos.Count > 20)
                        {
                            for (int i = 20; i < fotos.Count; i++)
                            {
                                if(fotos[i] != null)
                                    restoFotos.Add(fotos[i]);
                                else
                                    break;
                            }
                        }

                        /*string[] primerasFotos = new string[12];
                        string[] restoFotos = new string[fotos.Length - 12];
                        int resto = fotos.Length - 12;
                        for (int i = 0; i < 12; i++)
                            primerasFotos[i] = fotos[i];
                        for (int i = 0; i < resto; i++)
                            restoFotos[i] = fotos[i + 12];*/
                        doc.Add(TablaFotografias(primerasFotos));
                        doc.NewPage();
                        doc.Add(TablaFotografias(restoFotos));
                    }
                    foreach (var img in Directory.GetFiles(directoryImgs))
                    {
                        if(img.Contains("temp"))
                            File.Delete(img);
                    }
                    PdfContentByte cb = writer.DirectContent;
                    PdfPTable tablaObservaciones = TablaObservaciones();
                    tablaObservaciones.WriteSelectedRows(0, -1, 30, 275, cb);
                    PdfPTable tablaFirmas = TablaFirmas();
                    tablaFirmas.WriteSelectedRows(0, -1, 30, 180, cb);
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

        public void CeldasVacias(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell() { Border = 0 });
        }

        private IElement TablaEncabezado()
        {
            try
            {

                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 16.67f, 16.67f, 16.67f, 16.67f, 16.67f, 16.67f }) { WidthPercentage = 100f };
                CeldasVacias(6, table);

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
                var colTitulo = new PdfPCell(new Phrase(textoTitulo, letraoNegritaGrande)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingRight = 10, PaddingLeft = 10, Colspan = 6 };
                table.AddCell(colTitulo);

                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: TablaEncabezado", 3);
                return null;
            }

        }

        private IElement TablaFotografias(List<string> rutas)
        {

            try
            {
                int cuadros = 0;
                PdfPTable table;
                if (rutas.Count <= 4)
                {
                    table = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100f };
                    cuadros = 4;
                }
                else if (rutas.Count > 4 && rutas.Count <= 6)
                {
                    table = new PdfPTable(new float[] { 33.33f, 33.33f, 33.33f }) { WidthPercentage = 100f };
                    cuadros = 6;
                }
                else if (rutas.Count > 6 && rutas.Count <= 12)
                {
                    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                    cuadros = 12;
                }
                else if (rutas.Count > 12 && rutas.Count <= 16)
                {
                    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                    cuadros = 16;
                }
                else if (rutas.Count > 16 && rutas.Count <= 20)
                {
                    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                    cuadros = 20;
                }
                else
                {
                    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                    cuadros = 20;
                }

                foreach (var foto in rutas)
                {
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
                        }
                    }
                    if(!File.Exists(fotoTemporal))
                        imageReview.Save(fotoTemporal);
                    Image img = Image.GetInstance(fotoTemporal);
                    if (cuadros == 4)
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(180f, 140f);
                        else
                            img.ScaleAbsolute(140f, 180f);
                    }
                    else if (cuadros == 6)
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(130f, 140f);
                        else
                            img.ScaleAbsolute(140f, 130f);
                    }
                    else 
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(100f, 110f);
                        else
                            img.ScaleAbsolute(110f, 100f);
                    }
                    PdfPCell colFoto = new PdfPCell(img) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                    table.AddCell(colFoto);
                }
                for (int i = 0; i < cuadros - rutas.Count; i++)
                {
                    Image logo = Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\sinImagen.png");
                    logo.ScaleAbsolute(90f, 110f);

                    if (cuadros == 4)
                        logo.ScaleAbsolute(170f, 130f);
                    else if (cuadros == 6)
                        logo.ScaleAbsolute(120f, 130f);
                    else 
                        logo.ScaleAbsolute(100f, 110f);
                    PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                    table.AddCell(colLogo);
                }
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

                var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };

                string valorReporte = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["NumeroReporte"]) : Convert.ToString(_tableHeader.Rows[0]["Referencia"]);
                var colNoReporte = new PdfPCell(new Phrase(valorReporte, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };

                //TODO cambiar valor de fecha en Reporte fotográfico según el stored de Alex
                string valorFecha = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Fecha"]).Substring(0, 10) : "01/01/2021 ";


                table.AddCell(colTextoNoReporte);
                table.AddCell(colNoReporte);
                CeldasVacias(2, table);
                var colTextoFecha = new PdfPCell(new Phrase("Fecha: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };
                var colFecha = new PdfPCell(new Phrase(valorFecha, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };
                table.AddCell(colTextoFecha);
                table.AddCell(colFecha);
                CeldasVacias(1, table);

                //Plaza de cobro

                var colPlazaDeCobro = new PdfPCell(new Phrase("Plaza de Cobro: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };

                string valorPlaza = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Plaza"]) : Convert.ToString(_tableHeader.Rows[0]["Plaza"]);
                var plazaDeCobro = new PdfPCell(new Phrase(valorPlaza, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };




                table.AddCell(colPlazaDeCobro);
                table.AddCell(plazaDeCobro);
                CeldasVacias(2, table);
                //TODO poner la hora inicio del stored de Alex en reporte fotográfico equipo nuevo y dañado
                string valorHoraInicio = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Inicio"]) : "12:00";
                var inicioDateTime = Convert.ToDateTime(valorHoraInicio);
                string conversionInicio = inicioDateTime.ToString("hh:mm tt", CultureInfo.CurrentCulture);
                var colTextoHoraInicio = new PdfPCell(new Phrase("Hora INICIO: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };
                var colHoraInicio = new PdfPCell(new Phrase(conversionInicio, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colTextoHoraInicio);
                table.AddCell(colHoraInicio);
                CeldasVacias(1, table);

                //Ubicación

                string valorUbicacion = _tipo == 1 ? _ubicacion : Convert.ToString(_tableHeader.Rows[0]["Ubicacion"]);
                var colUbicacion = new PdfPCell(new Phrase("Ubicación: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };

                var ubicacion = new PdfPCell(new Phrase(valorUbicacion, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };


                //TODO actualizar hora fin con el stored de Alex
                table.AddCell(colUbicacion);
                table.AddCell(ubicacion);
                CeldasVacias(2, table);
                string valorHoraFin = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Fin"]) : "13:00";
                var finDateTime = Convert.ToDateTime(valorHoraFin);
                string conversionFin = finDateTime.ToString("hh:mm tt", CultureInfo.CurrentCulture);
                var colTextoHoraFin = new PdfPCell(new Phrase("Hora FIN: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };
                var colHoraFin = new PdfPCell(new Phrase(conversionFin, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colTextoHoraFin);
                table.AddCell(colHoraFin);

                CeldasVacias(1, table);

                //Técnico
                var colTecnico = new PdfPCell(new Phrase("Técnico Responsable PROSIS: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 3 };
                string valorTecnicoProsis = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["TecnicoProsis"]) : Convert.ToString(_tableHeader.Rows[0]["Tecnico"]);
                var tecnico = new PdfPCell(new Phrase(valorTecnicoProsis, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                table.AddCell(colTecnico);
                table.AddCell(tecnico);
                CeldasVacias(2, table);


                //Personal CAPUFE

                string valorCapufe = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["PersonalCapufe"]) : Convert.ToString(_tableHeader.Rows[0]["PersonalCapufe"]);
                var colCapufe = new PdfPCell(new Phrase("Personal de Plaza de Cobro CAPUFE: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 3 };
                var capufe = new PdfPCell(new Phrase(valorCapufe, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                table.AddCell(colCapufe);
                table.AddCell(capufe);
                CeldasVacias(2, table);


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

        private PdfPTable TablaObservaciones()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                table.TotalWidth = 550;
                CeldasVacias(24, table);


                var colTextoObservaciones = new PdfPCell(new Phrase("Observaciones: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 2 };

                table.AddCell(colTextoObservaciones);
                CeldasVacias(14, table);

                var celdaObservaciones = SeparacionObservaciones((_tipo == 1) ? Convert.ToString(_tableHeader.Rows[0]["Observaciones"]) : Convert.ToString(_tableHeader.Rows[0]["Observation"]));
                int celdasTotalesObservaciones = 0;
                foreach (var linea in celdaObservaciones)
                {
                    celdasTotalesObservaciones += 1;
                    var celdaLinea = new PdfPCell(new Phrase(Convert.ToString(linea), letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                for (int i = 0; i < 4 - celdasTotalesObservaciones; i++)
                {
                    var celdaLinea = new PdfPCell(new Phrase("", letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                CeldasVacias(16, table);
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

        private List<string> SeparacionObservaciones(string observaciones)
        {
            List<string> lineaObservaciones = new List<string>();
            if (observaciones.Length <= 100)
            {
                lineaObservaciones.Add(observaciones);
                return lineaObservaciones;
            }

            char[] separadores = new char[]{
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
            }
            return lineaObservaciones;
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
                table.AddCell(new PdfPCell() { BorderWidthTop = 1, BorderWidthBottom = 0, BorderWidthLeft = 1, BorderWidthRight = 1, FixedHeight = 30 });
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 1, BorderWidthRight = 1, FixedHeight = 30 });
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 1, BorderWidthRight = 1, FixedHeight = 30 });
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
                CeldasVacias(1, table);
                table.AddCell(colPersonal);
                CeldasVacias(2, table);

                //
                var colProsis = new PdfPCell(new Phrase("Proyectos y Sistemas Informáticos S.A. de C.V.", letraoNormalChicaFirmas))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var colCapufe = new PdfPCell(new Phrase("CAPUFE", letraoNormalChicaFirmas))
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
                _apiLogger.WriteLog(_clavePlaza, ex, "MantenimientoPdfCreation: TablaFirmas", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MantenimientoPdfCreation: TablaFirmas", 3);
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
