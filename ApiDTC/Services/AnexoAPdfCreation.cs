﻿using System;
using System.IO;
using ApiDTC.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ApiDTC.Services
{
    public class AnexoAPdfCreation
    {

        #region Attributes
        private readonly string _clavePlaza;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructors

        public AnexoAPdfCreation(string clavePlaza, ApiLogger apiLogger)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
        }

        #endregion

        #region BaseFont

        public static string path = $@"{System.Environment.CurrentDirectory}\Media\Montserrat\";

        public static BaseFont BoldGrande = BaseFont.CreateFont(path + "Montserrat Black 900.ttf", BaseFont.WINANSI, true);
        public static BaseFont NormalMediana = BaseFont.CreateFont(path + "Montserrat Medium 500.ttf", BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(path + "Montserrat Bold 700.ttf", BaseFont.WINANSI, true);
        public static BaseFont NegritaMedianaIta = BaseFont.CreateFont(path + "Montserrat SemiBold Italic 600.ttf", BaseFont.WINANSI,true);

        public static BaseFont NegritaGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        //public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        //public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChicaSubAzul = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteLetrita = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteMini = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion
        #region iText.Font

        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(BoldGrande, 13f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

        //public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 13f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNormalChicaFirmas = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);

        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);

        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        //public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

        public static iTextSharp.text.Font letraNegritaItalicaMedianaRoja = new iTextSharp.text.Font(NegritaMedianaIta, 8f, iTextSharp.text.Font.BOLD, BaseColor.Red);
        public static iTextSharp.text.Font letraNegritaItalicaMediana = new iTextSharp.text.Font(NegritaMedianaIta, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);

        public static iTextSharp.text.Font letraNormalMedianaRoja = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Red);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion

        #region Methods
        public Response NewPdf(string folder)
        {
            string directory, filename, path;
            directory = $@"{folder}\{_clavePlaza.ToUpper()}\Anexo";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            filename = $"AnexoA.pdf";

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
                _apiLogger.WriteLog(_clavePlaza, ex, "AnexoAPdfCreation: NewPdf", 2);
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
                    float margenLados = iTextSharp.text.Utilities.MillimetersToPoints(20f);
                    float margenTop = iTextSharp.text.Utilities.MillimetersToPoints(8.9f);
                    float margenBottom = iTextSharp.text.Utilities.MillimetersToPoints(30.0f);
                    float xCarta = iTextSharp.text.Utilities.MillimetersToPoints(215.9f);
                    float yCarta = iTextSharp.text.Utilities.MillimetersToPoints(279.4f);

                    doc.SetPageSize(new Rectangle(xCarta, yCarta));
                    doc.SetMargins(margenLados, margenLados, margenTop, margenBottom);
                    doc.AddAuthor("PROSIS");
                    doc.AddTitle("ANEXO A ACTA ADMINISTRATIVA INFORMATIVA (ENTREGA-RECEPCIÓN)");


                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelperVerticalAnexo();
                    writer.Open();
                    doc.Open();

                    doc.Add(TablaLegal());
                    doc.Add(TablaComponentesDañados());
                    doc.Add(TablaComponentesNuevos());
                    doc.Add(CierreFecha());

                    doc.NewPage();

                    TablaFirmas(doc);

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
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: NewPdf", 2);
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

        private IElement TablaLegal()
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };
                
                Chunk Uno = new Chunk("SE LEVANTA LA PRESENTE ACTA, PARA HACER CONSTAR LA SUSTITUCIÓN DE COMPONENTES POR FÍN DE VIDA ÚTIL/REUBICACIÓN/ACONDICIONAMIENTO, REALIZADO AL EQUIPO DE CONTROL DE TRANSITO DE PLAZA, EN LA PLAZA DE COBRO No.103, PALO BLANCO, PERTENECIENTE A LA UNIDAD REGIONAL CUERNAVACA.", letraNormalMediana);
                var parrafoUno = new Paragraph();
                parrafoUno.SetLeading(0, 1.4f);
                parrafoUno.Add(Uno);
                parrafoUno.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Dos = new Chunk("EN LA CIUDAD DE MAZATLAN, GRO., SIENDO LAS 17:30 HRS. DEL DÍA 20 DE DICIEMBRE DE 2021, EL LIC. JESUS GERMAIN GONZALEZ MORALES, DE LA PLAZA DE COBRO, EL C. JHOVANY MOHAMED ISAURO GARCIA, TÉCNICO REPRESENTANTE DE LA EMPRESA PRESTADORA DE SERVICIOS PROYECTOS Y SISTEMAS INFORMATICOS S.A. DE C.V. Y LOS C. FRANCISCO VALENTE MIRANDA Y EL C. JUAN SANTOS JIMENEZ, TESTIGOS DE ASISTENCIA, PARA HACER CONSTAR LA SUSTITUCIÓN DE COMPONENTES DEL EQUIPO DE CONTROL DE TRANSITO DE PLAZA, DE ACUERDO A LA SOLICITUD DE FECHA 15 DE DICIEMBRE DE 2021 Y AUTORIZADA EN OFICIO DO/3828/2021, DE FECHA 15 DE DICIEMBRE DE 2021, POR LA GERENCIA DE MANTENIMIENTO Y MODERNIZACIÓN DE EQUIPOS DE PEAJE; PARA CUYO EFECTÓ FUÉ NECESARIO REPONER EN FECHA 20 DE DICIEMBRE DE 2021 LAS PARTES QUE A CONTINUACIÓN SE DETALLAN.", letraNormalMediana);
                var parrafoDos = new Paragraph();
                parrafoDos.SetLeading(0, 1.4f);
                parrafoDos.Add(Dos);
                parrafoDos.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Tres = new Chunk("LOS EQUIPOS/COMPONENTES DAÑADOS EL ADMINISTRADOR DEBERÁ IDENTIFICAR Y EMBALAR, ENVIANDOLOS EN UN PERÍODO DE 5 DÍAS MÁXIMO AL ÁLMACÉN DE LA COORDINACION REGIONAL POR OFICIO, PARA SU DESTINO FINAL CONFORME LA NORMA CAPUFE.", letraNormalMediana);
                var parrafoTres = new Paragraph();
                parrafoTres.SetLeading(0, 1.4f);
                parrafoTres.Add(Tres);
                parrafoTres.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Cuatro = new Chunk("LOS TRABAJOS, INSTALACIÓN Y OPERACIÓN DE LA PLAZA DE COBRO QUEDAN A ENTERA SATISFACCIÓN DEL ADMINISTRADOR DE LA PLAZA DE COBRO Y/O ENCARGADO DE TURNO.", letraNormalMediana);
                var parrafoCuatro = new Paragraph();
                parrafoCuatro.SetLeading(0, 1.4f);
                parrafoCuatro.Add(Cuatro);
                parrafoCuatro.Alignment = Element.ALIGN_JUSTIFIED;

                var colParrafoUno = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 20,
                    PaddingBottom = 10,
                };
                colParrafoUno.AddElement(parrafoUno);

                var colParrafoDos = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10
                };
                colParrafoDos.AddElement(parrafoDos);

                var colParrafoTres = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10
                };
                colParrafoTres.AddElement(parrafoTres);

                var colParrafoCuatro = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10
                };
                colParrafoCuatro.AddElement(parrafoCuatro);

                table.AddCell(colParrafoUno);
                table.AddCell(colParrafoDos);
                table.AddCell(colParrafoTres);
                table.AddCell(colParrafoCuatro);

                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 3);
                return null;
            }

        }

        private IElement TablaComponentesDañados()
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 14.28f, 14.28f, 14.28f, 14.28f, 14.28f, 14.28f, 14.28f }) { WidthPercentage = 100f };

                Chunk ComponenteDañado = new Chunk("COMPONENTES Y/O REFACCIONES DAÑADAS:", letraoNegritaMediana);
                ComponenteDañado.SetUnderline(1, -1);
                var parrafoComponenteDañado = new Paragraph();
                parrafoComponenteDañado.Add(ComponenteDañado);
                parrafoComponenteDañado.Alignment = Element.ALIGN_LEFT;

                var encabezadoComponenteDañado = new PdfPCell() { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 15, Colspan = 7 };
                encabezadoComponenteDañado.AddElement(ComponenteDañado);

                var encabezadoCantidad = new PdfPCell(new Phrase("Cantidad", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoComponente = new PdfPCell(new Phrase("Componente", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoMarca = new PdfPCell(new Phrase("Marca Y/O Modelo", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoSerie = new PdfPCell(new Phrase("No. de Serie", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoInventario = new PdfPCell(new Phrase("No. Inventario", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoUbicacion = new PdfPCell(new Phrase("Ubicacion", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoObservaciones = new PdfPCell(new Phrase("Observaciones", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };

                var espacioVacio = new PdfPCell(new Phrase("", letraoNegritaMediana)) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 0, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 15, Colspan = 7 };

                table.AddCell(encabezadoComponenteDañado);
                table.AddCell(encabezadoCantidad);
                table.AddCell(encabezadoComponente);
                table.AddCell(encabezadoMarca);
                table.AddCell(encabezadoSerie);
                table.AddCell(encabezadoInventario);
                table.AddCell(encabezadoUbicacion);
                table.AddCell(encabezadoObservaciones);

                CeldasContenido(7, table);
                table.AddCell(espacioVacio);

                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 3);
                return null;
            }

        }

        private IElement TablaComponentesNuevos()
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 14.28f, 14.28f, 14.28f, 14.28f, 14.28f, 14.28f, 14.28f }) { WidthPercentage = 100f };

                Chunk ComponenteDañado = new Chunk("COMPONENTES Y/O REFACCIONES NUEVAS:", letraoNegritaMediana);
                ComponenteDañado.SetUnderline(1, -1);
                var parrafoComponenteDañado = new Paragraph();
                parrafoComponenteDañado.Add(ComponenteDañado);
                parrafoComponenteDañado.Alignment = Element.ALIGN_LEFT;

                var encabezadoComponenteDañado = new PdfPCell() { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 15, Colspan = 7 };
                encabezadoComponenteDañado.AddElement(ComponenteDañado);

                var encabezadoCantidad = new PdfPCell(new Phrase("Cantidad", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoComponente = new PdfPCell(new Phrase("Componente", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoMarca = new PdfPCell(new Phrase("Marca Y/O Modelo", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoSerie = new PdfPCell(new Phrase("No. de Serie", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoInventario = new PdfPCell(new Phrase("No. Inventario", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoUbicacion = new PdfPCell(new Phrase("Ubicacion", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                var encabezadoObservaciones = new PdfPCell(new Phrase("Observaciones", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };

                var espacioVacio = new PdfPCell(new Phrase("", letraoNegritaMediana)) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 0, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 15, Colspan = 7 };

                table.AddCell(encabezadoComponenteDañado);
                table.AddCell(encabezadoCantidad);
                table.AddCell(encabezadoComponente);
                table.AddCell(encabezadoMarca);
                table.AddCell(encabezadoSerie);
                table.AddCell(encabezadoInventario);
                table.AddCell(encabezadoUbicacion);
                table.AddCell(encabezadoObservaciones);

                CeldasContenido(7, table);
                table.AddCell(espacioVacio);

                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 3);
                return null;
            }

        }

        private IElement CierreFecha()
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };

                string ParrafoUno = "Se cierra la presente acta en fecha 20 de diciembre de 2021 siendo las 18:00 hrs.";

                var colParrafoUno = new PdfPCell(new Phrase(ParrafoUno, letraoNegritaMediana))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingBottom = 10
                };
                table.AddCell(colParrafoUno);
 
                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 3);
                return null;
            }
        }

        private IElement TablaFirmas(Document doc)
        {
            try
            {
                PdfPTable tablaCuatroFirmas = new PdfPTable(new float[] { 22.5f, 3.333f, 22.5f, 3.333f, 22.5f, 3.333f, 22.5f }) { WidthPercentage = 100f };

                /////////////////////Encabezado con firma/////////////////////////////////////

                Chunk porCapufe = new Chunk("POR CAPUFE", letraoNegritaMediana);
                var parrafoCapufe = new Paragraph();
                parrafoCapufe.SetLeading(0, 1.2f);
                parrafoCapufe.Add(porCapufe);
                parrafoCapufe.Alignment = Element.ALIGN_CENTER;

                Chunk porEmpresa = new Chunk("POR LA EMPRESA", letraoNegritaMediana);
                var parrafoEmpresa = new Paragraph();
                parrafoEmpresa.SetLeading(0, 1.2f);
                parrafoEmpresa.Add(porEmpresa);
                parrafoEmpresa.Alignment = Element.ALIGN_CENTER;

                Chunk admin = new Chunk("ADMINISTRADOR Y ENCARGADO DE LA SUPERINTENDENCIA DE OPERACIÓN", letraoNegritaMediana);
                var parrafoAdmin = new Paragraph();
                parrafoAdmin.SetLeading(0, 1.2f);
                parrafoAdmin.Add(admin);
                parrafoAdmin.Alignment = Element.ALIGN_CENTER;

                Chunk subg = new Chunk("SUBGERENTE DE OPERACIÓN CUERNAVACA", letraoNegritaMediana);
                var parrafoSubgerente = new Paragraph();
                parrafoSubgerente.SetLeading(0, 1.2f);
                parrafoSubgerente.Add(subg);
                parrafoSubgerente.Alignment = Element.ALIGN_CENTER;

                var capufe = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 30
                };
                capufe.AddElement(parrafoCapufe);

                var empresa = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 30
                };
                empresa.AddElement(parrafoEmpresa);

                var administrador = new PdfPCell(new Phrase())
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 30
                };
                administrador.AddElement(parrafoAdmin);

                var subgerente = new PdfPCell(new Phrase())
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 30
                };
                subgerente.AddElement(parrafoSubgerente);

                /////////////////////////////////Nombres////////////////////////////////////
                ///

                Chunk nomCapufe = new Chunk("LIC. JESÚS GERMAIN GONZÁLEZ MORALES.\n Administrador o Encargado de Turno\n AUTORIZÓ", letraoNegritaMediana);
                var parrafonomCapufe = new Paragraph();
                parrafonomCapufe.SetLeading(0, 1.8f);
                parrafonomCapufe.Add(nomCapufe);
                parrafonomCapufe.Alignment = Element.ALIGN_CENTER;

                Chunk nomEmpresa = new Chunk("C. JHOVANY MOHAMED ISAURO GARCIA\n Técnico\n REALIZÓ", letraoNegritaMediana);
                var parrafonomEmpresa = new Paragraph();
                parrafonomEmpresa.SetLeading(0, 1.8f);
                parrafonomEmpresa.Add(nomEmpresa);
                parrafonomEmpresa.Alignment = Element.ALIGN_CENTER;

                Chunk nomAdministrador = new Chunk("C. CARLOS BÁRCENAS ORTEGA\n SUPERVISÓN", letraoNegritaMediana);
                var parrafonomAdministrador = new Paragraph();
                parrafonomAdministrador.SetLeading(0, 1.8f);
                parrafonomAdministrador.Add(nomAdministrador);
                parrafonomAdministrador.Alignment = Element.ALIGN_CENTER;

                Chunk nomSubg = new Chunk("L.A.E. FIDEL URIBE HERNÁNDEZ\n Vo.Bo.", letraoNegritaMediana);
                var parrafonomSubg = new Paragraph();
                parrafonomSubg.SetLeading(0, 1.8f);
                parrafonomSubg.Add(nomSubg);
                parrafonomSubg.Alignment = Element.ALIGN_CENTER;

                var nombreCapufe = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };
                nombreCapufe.AddElement(parrafonomCapufe);

                var nombreEmpresa = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };
                nombreEmpresa.AddElement(parrafonomEmpresa);

                var nombreAdministrador = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };
                nombreAdministrador.AddElement(parrafonomAdministrador);

                var nombreSubgerente = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };
                nombreSubgerente.AddElement(parrafonomSubg);

                var espacioVacio = new PdfPCell(new Phrase("", letraoNegritaMediana)) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 0, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 15, Colspan = 7 };
                tablaCuatroFirmas.AddCell(capufe);
                CeldasVacias(1, tablaCuatroFirmas);
                tablaCuatroFirmas.AddCell(empresa);
                CeldasVacias(1, tablaCuatroFirmas);
                tablaCuatroFirmas.AddCell(administrador);
                CeldasVacias(1, tablaCuatroFirmas);
                tablaCuatroFirmas.AddCell(subgerente);

                tablaCuatroFirmas.AddCell(nombreCapufe);
                CeldasVacias(1, tablaCuatroFirmas);
                tablaCuatroFirmas.AddCell(nombreEmpresa);
                CeldasVacias(1, tablaCuatroFirmas);
                tablaCuatroFirmas.AddCell(nombreAdministrador);
                CeldasVacias(1, tablaCuatroFirmas);
                tablaCuatroFirmas.AddCell(nombreSubgerente);

                tablaCuatroFirmas.AddCell(espacioVacio);

                PdfPTable tablaDosFirmas = new PdfPTable(new float[] { 13.33f, 30f, 13.33f, 30f, 13.33f }) { WidthPercentage = 100f };

                /////////////////////////////////////////////////testigos/////////////////////////////////////////////

                Chunk nomEncargadoUno = new Chunk("FRANCISCO VALENTE\n Encargado de turno", letraoNegritaMediana);
                var parrafonomEncargadoUno = new Paragraph();
                parrafonomEncargadoUno.SetLeading(0, 1.8f);
                parrafonomEncargadoUno.Add(nomEncargadoUno);
                parrafonomEncargadoUno.Alignment = Element.ALIGN_CENTER;

                Chunk nomEncargadoDos = new Chunk("JUAN SANTOS JIMENEZ\n Encargado de turno", letraoNegritaMediana);
                var parrafonomEncargadoDos = new Paragraph();
                parrafonomEncargadoDos.SetLeading(0, 1.8f);
                parrafonomEncargadoDos.Add(nomEncargadoDos);
                parrafonomEncargadoDos.Alignment = Element.ALIGN_CENTER;

                var lineaDeFirmas = new PdfPCell(new Phrase("", letraoNegritaMediana))
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 50
                };

                var encargadoUno = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };
                encargadoUno.AddElement(parrafonomEncargadoUno);

                var encabezadosTestigos = new PdfPCell(new Phrase("TESTIGOS:", letraoNegritaMediana))
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_TOP,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };

                var encargadoDos = new PdfPCell()
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };
                encargadoDos.AddElement(parrafonomEncargadoDos);

                var espacioVacio2 = new PdfPCell(new Phrase("", letraoNegritaMediana)) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 0, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 50, Colspan = 5 };

                CeldasVacias(1, tablaDosFirmas);
                tablaDosFirmas.AddCell(lineaDeFirmas);
                tablaDosFirmas.AddCell(encabezadosTestigos);
                tablaDosFirmas.AddCell(lineaDeFirmas);
                CeldasVacias(1, tablaDosFirmas);

                CeldasVacias(1, tablaDosFirmas);
                tablaDosFirmas.AddCell(encargadoUno);
                CeldasVacias(1, tablaDosFirmas);
                tablaDosFirmas.AddCell(encargadoDos);
                CeldasVacias(1, tablaDosFirmas);

                tablaDosFirmas.AddCell(espacioVacio2);

                PdfPTable tablaPlaza = new PdfPTable(new float[] { 23.33f, 10f, 23.33f, 10f, 23.33f }) { WidthPercentage = 100f };

                string ParrafoEncabezado = "SELLO DE PLAZA DE COBRO";

                Chunk parrafoS = new Chunk("REQUISITAR ESTRICTAMENTE, LA ENTREGA Y RECEPCIÓN DE LA PRESENTE EN ORIGINAL POR LA SUBGERENCIA DE OPERACIÓN DE LA COORDINACIÓN Y PRESTADOR DE SERVICIO (EMPRESA) “RUBRICA” EN TODAS LAS PÁGINAS.", letraoNegritaMediana);
                var parrafoSello = new Paragraph();
                parrafoSello.SetLeading(0, 1.4f);
                parrafoSello.Add(parrafoS);
                parrafoSello.Alignment = Element.ALIGN_JUSTIFIED;

                var colEncabezado = new PdfPCell(new Phrase(ParrafoEncabezado, letraoNegritaMediana))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 15,
                    PaddingBottom = 20,
                    Colspan = 5

                };

                var colParrafoUno = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 15,
                    PaddingBottom = 20,
                    Colspan = 5
                };
                colParrafoUno.AddElement(parrafoSello);

                var fecha = new PdfPCell(new Phrase("FECHA DE ENTREGA", letraoNegritaMediana))
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 50
                };

                var entrega = new PdfPCell(new Phrase("ENTREGA", letraoNegritaMediana))
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 50
                };

                var recibe = new PdfPCell(new Phrase("RECIBE", letraoNegritaMediana))
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 50
                };

                var nombreYfirma = new PdfPCell(new Phrase("Nombre y firma de CAPUFE", letraoNegritaMediana))
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };

                var nombreYfirmaTecnico = new PdfPCell(new Phrase("Nombre y firma técnico de la empresa", letraoNegritaMediana))
                {
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    PaddingBottom = 5
                };

                tablaPlaza.AddCell(colEncabezado);
                tablaPlaza.AddCell(colParrafoUno);
                tablaPlaza.AddCell(fecha);
                CeldasVacias(1, tablaPlaza);
                tablaPlaza.AddCell(entrega);
                CeldasVacias(1, tablaPlaza);
                tablaPlaza.AddCell(recibe);
                CeldasVacias(2, tablaPlaza);
                tablaPlaza.AddCell(nombreYfirma);
                CeldasVacias(1, tablaPlaza);
                tablaPlaza.AddCell(nombreYfirmaTecnico);

                //////////////////////////////////////////////////////////
                ///
                PdfPTable tablaFinal = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };

                string LineaUnoFinal = "ESTRICTAMENTE LA SUBGERENCIA DE OPERACIÓN DEBERA ENTREGAR COPIA A:";
                string LineaDosFinal = "C.c.p.";
                string LineaTresFinal = "Lic. Rogelio Galicia Pérez. -Gerente de Seguros/ rgalicia@capufe.gob.mx";
                string LineaCuatroFinal = "C. Ricardo Esparza.- Subgerente de Almacenes e Inventarios /resparza@capufe.gob.mx";
                string LineaCincoFinal = "Superintendencia de Recursos Materiales de la Coordinación Regional";
                //No funcionaba la alineacion XD
                string LineaSeisFinal = "                                                                                     Para los fines que sean necesarios.";

                Chunk finalEncabezado = new Chunk(LineaUnoFinal, letraNegritaItalicaMedianaRoja);
                var parrafoFinalE = new Paragraph();
                parrafoFinalE.SetLeading(0, 2f);
                finalEncabezado.SetUnderline(1, -2);
                parrafoFinalE.Add(finalEncabezado);
                parrafoFinalE.Alignment = Element.ALIGN_CENTER;

                Chunk finalParrafo = new Chunk(LineaDosFinal + "\n" + LineaTresFinal + "\n" + LineaCuatroFinal + "\n" + LineaCincoFinal + "\n", letraNegritaItalicaMediana);
                var parrafoFinal = new Paragraph();
                parrafoFinal.SetLeading(0, 1.4f);
                parrafoFinal.Add(finalParrafo);
                parrafoFinal.Alignment = Element.ALIGN_LEFT;

                Chunk fines = new Chunk(LineaSeisFinal, letraNormalMediana);
                var parrafoFines = new Paragraph();
                parrafoFines.SetLeading(0, 1.4f);
                parrafoFines.Add(fines);
                parrafoFines.Alignment = Element.ALIGN_RIGHT;

                var colParrafoFinal = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_MIDDLE,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 15,
                    PaddingBottom = 5
                };
                colParrafoFinal.AddElement(parrafoFinalE);
                colParrafoFinal.AddElement(parrafoFinal);
                colParrafoFinal.AddElement(fines);

                tablaFinal.AddCell(colParrafoFinal);


                doc.Add(tablaCuatroFirmas);
                doc.Add(tablaDosFirmas);
                doc.Add(tablaPlaza);
                doc.Add(tablaFinal);

                return null;
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

        public void CeldasVacias(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell() { Border = 0 });
        }

        public void CeldasContenido(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell(new Phrase("S073940280147", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, PaddingBottom = 10, PaddingTop = 10 });
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
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: FileInUse", 5);
                return true;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: FileInUse", 3);
                return true;
            }
            return fileInUse;
        }
        #endregion

    }
 }
