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

        private readonly string _referenceAnexo;
        string directoryImageDiagnostico;   //las fotos del diagnostico son las mismas que en reporte equipo dañado
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

        public ReporteFotograficoPdfCreation(string clavePlaza, DataTable tableHeader, ApiLogger apiLogger, int tipo, string referenceNumber, string ubicacion, string referenceAnexo = "")
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _tipo = tipo;
            _referenceNumber = referenceNumber;
            _ubicacion = ubicacion;
            _referenceAnexo = referenceAnexo;
        }

        #endregion
        //https://localhost:44358/api/ReporteFotografico/Reporte/JOR/B01
        #region Methods

        public Response NewPdf(string folder)
        {
            string directory, filename, path;

            if (_tipo == 1)
                directory = $@"{folder}\{_clavePlaza.ToUpper()}\Reportes\{_referenceNumber}";
            else if (_tipo == 2)
                directory = $@"{folder}\{_clavePlaza.ToUpper()}\DTC\{_referenceNumber}\Reportes Fotograficos Equipo Nuevo\{_tableHeader.Rows[0]["AnexoReference"]}";
            else
                directory = $@"{folder}\{_clavePlaza.ToUpper()}\DTC\{_referenceNumber}";
            if (!Directory.Exists(directory) && _tipo != 2)
                return new Response
                {
                    Message = "Error: No existe el directorio",
                    Result = null
                };
            else
                Directory.CreateDirectory($@"{folder}\{_clavePlaza.ToUpper()}\DTC\{_referenceNumber}\Reportes Fotograficos Equipo Nuevo\{_tableHeader.Rows[0]["AnexoReference"]}");

            if (_tipo == 1)
                filename = $"ReporteFotográfico-{_referenceNumber}.pdf";
            else if (_tipo == 2)
                filename = $@"DTC-{_referenceNumber}-EquipoNuevo.pdf";
            else
                filename = $@"DTC-{_referenceNumber}-EquipoDañado.pdf";
            if (_tipo == 3)
            {
                //usar el _referenceNumber para crear la estructura con DT
                directoryImageDiagnostico = $@"{folder}\{_clavePlaza.ToUpper()}\Reportes\" + _tableHeader.Rows[0]["DiagnosisReference"] + "\\";
            }
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

                    //codemcm: Agregar fotos aqui!!!
                    doc.Add(TablaEncabezado());
                    doc.Add(TablaInformacion());
                    string directoryImgs;
                    if (_tipo == 1)
                        directoryImgs = Path.Combine(directory, "Imgs");
                    else if (_tipo == 2)
                        directoryImgs = Path.Combine(directory, "Imgs");
                    else
                        directoryImgs = Path.Combine(directoryImageDiagnostico, "DiagnosticoFallaImgs");//Misma carpeta que Diagnostico
                    if (Directory.Exists(directoryImgs))
                    {
                        var files = Directory.GetFiles(directoryImgs);
                        if (files.Length != 0)
                        {
                            List<string> fotos = new List<string>();
                            foreach (var file in files)
                                fotos.Add(file);

                            List<string> fotosEnPagina = new List<string>();
                            PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };
                            PdfPTable table2 = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };
                            var celdaVacia = new PdfPCell() { Border = 0, Padding = 35f };
                            var celdaVacia2 = new PdfPCell() { Border = 0, Padding = 7f };
                            int pagina;
                            int i = 0;

                            if (fotos.Count <= 12)
                            {
                                pagina = 1;
                                table.AddCell(celdaVacia2);
                                doc.Add(table);
                                doc.Add(TablaFotografias(fotos, pagina));
                            }
                            else
                            {
                                while (i < fotos.Count)
                                {
                                    fotosEnPagina.Add(fotos[i]);

                                    if (i == 11)
                                    {
                                        pagina = 2;
                                        table.AddCell(celdaVacia);
                                        doc.Add(table);
                                        doc.Add(TablaFotografias(fotosEnPagina, pagina));
                                        fotosEnPagina.Clear();
                                    }
                                    if ((i + 1) % 12 == 0 && i > 12)
                                    {
                                        doc.NewPage();
                                        if ((i + 1) == fotos.Count)
                                        {
                                            pagina = 4;
                                            table2.AddCell(celdaVacia);
                                            doc.Add(table2);
                                        }
                                        else
                                        {
                                            table.AddCell(celdaVacia);
                                            doc.Add(table);
                                            pagina = 3;
                                        }
                                        doc.Add(TablaFotografias(fotosEnPagina, pagina));
                                        fotosEnPagina.Clear();
                                    }
                                    i++;
                                }
                                if (i % 12 != 0)
                                {
                                    pagina = 4;
                                    doc.NewPage();
                                    table2.AddCell(celdaVacia2);
                                    doc.Add(table2);
                                    doc.Add(TablaFotografias(fotosEnPagina, pagina));
                                    fotosEnPagina.Clear();
                                }
                            }
                        }

                        foreach (var img in Directory.GetFiles(directoryImgs))

                        {

                            if (img.Contains("temp"))

                                File.Delete(img);

                        }
                    }
                    PdfContentByte cb = writer.DirectContent;

                    string textoObservaciones = Convert.ToString((_tipo == 1) ? Convert.ToString(_tableHeader.Rows[0]["Observaciones"]) : Convert.ToString(_tableHeader.Rows[0]["Observation"]));
                    //Pendiente introducir texto
                    string textoObservacionesNuevo = "";
                    if (_tipo == 2)
                        textoObservacionesNuevo = _tableHeader.Rows[0]["Observaciones"].ToString();
                    AgregarObservaciones tabla = _tipo == 2 ? new AgregarObservaciones(new ApiLogger(), textoObservacionesNuevo, "Observaciones: ", _clavePlaza, "ReporteFotograficoPdfCreation: TablaObservaciones", 5, 3) : new AgregarObservaciones(new ApiLogger(), textoObservaciones, "Observaciones: ", _clavePlaza, "ReporteFotograficoPdfCreation: TablaObservaciones", 5, 3);
                    tabla.TablaObservaciones().WriteSelectedRows(0, -1, 30, 275, cb);


                    PdfPTable tablaFirmas = TablaFirmas();
                    tablaFirmas.WriteSelectedRows(0, -1, 30, 180, cb);
                    doc.Close();
                    writer.Close();

                    byte[] content = myMemoryStream.ToArray();

                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(content, 0, (int)content.Length);
                    }

                    VerificarPDF thepdf = new VerificarPDF();

                    if (thepdf.IsPDFOk(path).Equals(false))
                    {
                        throw new PdfException();
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
            catch (PdfException ex)
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                _apiLogger.WriteLog(_clavePlaza, ex, "ReporteFotograficoPdfCreation: NewPdf", 5);
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

        private IElement TablaFotografias(List<string> rutas, int pagina)
        {

            try
            {
                int cuadros = 0;
                PdfPTable table;
                if (rutas.Count == 9)
                {
                    table = new PdfPTable(new float[] { 33.33f, 33.33f, 33.33f }) { WidthPercentage = 100f };
                    cuadros = 9;
                }
                else if (rutas.Count <= 4)
                {
                    table = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100f };
                    cuadros = 4;
                }
                else if (rutas.Count > 4 && rutas.Count <= 6)
                {
                    table = new PdfPTable(new float[] { 33.33f, 33.33f, 33.33f }) { WidthPercentage = 100f };
                    cuadros = 6;
                }
                //else if (rutas.Count > 6 && rutas.Count <= 12)
                //{
                //    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                //    cuadros = 12;
                //}
                else //if (rutas.Count > 12 && rutas.Count <= 16)
                {
                    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                    cuadros = 12;
                }
                //else if (rutas.Count > 16 && rutas.Count <= 20)
                //{
                //    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                //    cuadros = 20;
                //}
                //else
                //{
                //    table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                //    cuadros = 20;
                //}

                foreach (var foto in rutas)
                {
                    //Si procesa un archivo temporal que no se eliminó
                    if (foto.Contains("-temp"))
                    {
                        File.Delete(foto);
                        continue;
                    }
                    System.Drawing.Image imageReview = System.Drawing.Image.FromFile(foto);
                    string fotoTemporal = foto.Substring(0, foto.LastIndexOf('.')) + "-temp.jpg";
                    foreach (var prop in imageReview.PropertyItems)
                    {
                        if (prop.Id == 0x0112)
                        {
                            int orientationValue = imageReview.GetPropertyItem(prop.Id).Value[0];
                            System.Drawing.RotateFlipType rotateFlipType = GetOrientationToFlipType(orientationValue);
                            imageReview.RotateFlip(rotateFlipType);
                            imageReview.RemovePropertyItem(0x0112);
                            if (!File.Exists(fotoTemporal))
                                File.Delete(fotoTemporal);
                            imageReview.Save(fotoTemporal);
                            break;
                        }
                    }
                    if (!File.Exists(fotoTemporal))
                        imageReview.Save(fotoTemporal);
                    Image img = Image.GetInstance(fotoTemporal);
                    PlantillasImagenes(img, cuadros, pagina);
                    PdfPCell colFoto = new PdfPCell(img) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                    table.AddCell(colFoto);
                }
                for (int i = 0; i < cuadros - rutas.Count; i++)
                {
                    Image logo = Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\sinImagen.png");
                    logo.ScaleAbsolute(90f, 110f);
                    PlantillasImagenes(logo, cuadros, pagina);
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

        private IElement PlantillasImagenes(Image img, int cuadros, int pagina)
        {
            //Caso 1: Primera pagina con observaciones
            switch (pagina)
            {
                case 1:
                    if (cuadros == 4)
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(160f, 140f);
                        else
                            img.ScaleAbsolute(140f, 160f);
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
                    break;
                //Caso 2: Primera pagina sin observaciones
                case 2:
                    if (cuadros == 4)
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(160f, 140f);
                        else
                            img.ScaleAbsolute(140f, 160f);
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
                    break;
                //Caso 3: Pagina intermedia
                case 3:
                    if (cuadros == 4)
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(160f, 140f);
                        else
                            img.ScaleAbsolute(150f, 170f);
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
                    break;
                //Caso 4: Ultima pagina
                case 4:
                    if (cuadros == 4)
                    {
                        if (img.Width > img.Height)
                            img.ScaleAbsolute(160f, 140f);
                        else
                            img.ScaleAbsolute(140f, 160f);
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
                    break;
            }
            return null;
        }

        private IElement TablaInformacion()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                CeldasVacias(8, table);
                var celdaVacia = new PdfPCell() { Border = 0 };

                var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };

                string valorReporte = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["NumeroReporte"]) : Convert.ToString(_tableHeader.Rows[0]["Referencia"]);
                var colNoReporte = new PdfPCell(new Phrase(valorReporte, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                //TODO cambiar valor de fecha en Reporte fotográfico según el stored de Alex
                string valorFecha = Convert.ToString(_tableHeader.Rows[0]["Fecha"]).Substring(0, 10);


                table.AddCell(colTextoNoReporte);
                table.AddCell(colNoReporte);
                //CeldasVacias(2, table);
                var colTextoFecha = new PdfPCell(new Phrase("Fecha: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };
                var colFecha = new PdfPCell(new Phrase(valorFecha, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };
                table.AddCell(colTextoFecha);
                table.AddCell(colFecha);
                CeldasVacias(1, table);

                //Plaza de cobro

                var colPlazaDeCobro = new PdfPCell(new Phrase("Plaza de Cobro: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };

                string valorPlaza = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["Plaza"]) : Convert.ToString(_tableHeader.Rows[0]["Plaza"]);
                var plazaDeCobro = new PdfPCell(new Phrase(valorPlaza, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };




                table.AddCell(colPlazaDeCobro);
                table.AddCell(plazaDeCobro);
                //CeldasVacias(2, table);
                //TODO poner la hora inicio del stored de Alex en reporte fotográfico equipo nuevo y dañado
                string valorHoraInicio = _tipo == 2 ? Convert.ToString(_tableHeader.Rows[0]["FechaApertura"]) : Convert.ToString(_tableHeader.Rows[0]["Inicio"]);
                //var inicioDateTime = Convert.ToDateTime(valorHoraInicio);
                //string conversionInicio = inicioDateTime.ToString("hh:mm tt", CultureInfo.CurrentCulture);
                var colTextoHoraInicio = new PdfPCell(new Phrase("Hora de inicio: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };
                var colHoraInicio = new PdfPCell(new Phrase(valorHoraInicio, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };

                table.AddCell(colTextoHoraInicio);
                table.AddCell(colHoraInicio);
                CeldasVacias(1, table);

                //Ubicación

                string valorUbicacion = _tipo == 1 ? _ubicacion : Convert.ToString(_tableHeader.Rows[0]["Ubicacion"]);
                var colUbicacion = new PdfPCell(new Phrase("Ubicación: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };

                var ubicacion = new PdfPCell(new Phrase(valorUbicacion, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };


                //TODO actualizar hora fin con el stored de Alex
                table.AddCell(colUbicacion);
                table.AddCell(ubicacion);
                //CeldasVacias(2, table);
                string valorHoraFin = _tipo == 2 ? Convert.ToString(_tableHeader.Rows[0]["FechaCierre"]) : Convert.ToString(_tableHeader.Rows[0]["Fin"]);
                //var finDateTime = Convert.ToDateTime(valorHoraFin);
                //string conversionFin = finDateTime.ToString("hh:mm tt", CultureInfo.CurrentCulture);
                var colTextoHoraFin = new PdfPCell(new Phrase("Hora de fin: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5 };
                var colHoraFin = new PdfPCell(new Phrase(valorHoraFin, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };

                table.AddCell(colTextoHoraFin);
                table.AddCell(colHoraFin);

                CeldasVacias(1, table);

                CeldasVacias(8, table);
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

                //No. De Siniestro
                //if (_tipo == 2)
                //{
                //    var colSiniestro = new PdfPCell(new Phrase("No De Siniestro: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 3 };
                //    string valorColSiniestro = Convert.ToString(_tableHeader.Rows[0]["NumeroSinisestro"]);
                //    var siniestro = new PdfPCell(new Phrase(valorColSiniestro, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                //    table.AddCell(colSiniestro);
                //    table.AddCell(siniestro);
                //    CeldasVacias(2, table);
                //}
                //No. De Oficio
                if (_tipo == 3)
                {
                    //1 Operacion
                    //2 Siniestro
                    //3 Fin de vida util
                    string nameColumn = "data";
                    if (Convert.ToInt32(_tableHeader.Rows[0]["TypeFaultId"]) == 2)
                    {
                        nameColumn = "No. Siniestro";
                    }
                    else if (Convert.ToInt32(_tableHeader.Rows[0]["TypeFaultId"]) == 3)
                    {
                        nameColumn = "Oficio";
                    }

                    var colSiniestro = new PdfPCell(new Phrase(nameColumn, letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 3 };
                    string valorColSiniestro = Convert.ToString(_tableHeader.Rows[0]["NumeroSiniestro"]);
                    var siniestro = new PdfPCell(new Phrase(valorColSiniestro, letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                    table.AddCell(colSiniestro);
                    table.AddCell(siniestro);
                    CeldasVacias(2, table);
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
