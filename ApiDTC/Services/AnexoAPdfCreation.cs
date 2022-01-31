using System;
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
                    doc.SetPageSize(new Rectangle(609.4488f, 793.701f));
                    doc.SetMargins(35f, 35f, 30f, 30f);
                    doc.AddAuthor("PROSIS");
                    doc.AddTitle("ANEXO A ACTA ADMINISTRATIVA INFORMATIVA (ENTREGA-RECEPCIÓN)");


                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelperVerticalAnexo();
                    writer.Open();
                    doc.Open();

                    doc.Add(TablaLegal());
                    doc.Add(TablaComponentes());
                    doc.Add(TablaComponentes());
                    doc.Add(CierreFecha());

                    doc.NewPage();

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

                string ParrafoUno = "SE LEVANTA LA PRESENTE ACTA, PARA HACER CONSTAR LA SUSTITUCIÓN DE COMPONENTES POR FÍN DE VIDA ÚTIL/REUBICACIÓN/ACONDICIONAMIENTO, REALIZADO AL EQUIPO DE CONTROL DE TRANSITO DE PLAZA, EN LA PLAZA DE COBRO No.103, PALO BLANCO, PERTENECIENTE A LA UNIDAD REGIONAL CUERNAVACA.";
                string ParrafoDos = "EN LA CIUDAD DE MAZATLAN, GRO., SIENDO LAS 17:30 HRS. DEL DÍA 20 DE DICIEMBRE DE 2021, EL LIC. JESUS GERMAIN GONZALEZ MORALES, DE LA PLAZA DE COBRO, EL C. JHOVANY MOHAMED ISAURO GARCIA, TÉCNICO REPRESENTANTE DE LA EMPRESA PRESTADORA DE SERVICIOS PROYECTOS Y SISTEMAS INFORMATICOS S.A. DE C.V. Y LOS C. FRANCISCO VALENTE MIRANDA Y EL C. JUAN SANTOS JIMENEZ, TESTIGOS DE ASISTENCIA, PARA HACER CONSTAR LA SUSTITUCIÓN DE COMPONENTES DEL EQUIPO DE CONTROL DE TRANSITO DE PLAZA, DE ACUERDO A LA SOLICITUD DE FECHA 15 DE DICIEMBRE DE 2021 Y AUTORIZADA EN OFICIO DO/3828/2021, DE FECHA 15 DE DICIEMBRE DE 2021, POR LA GERENCIA DE MANTENIMIENTO Y MODERNIZACIÓN DE EQUIPOS DE PEAJE; PARA CUYO EFECTÓ FUÉ NECESARIO REPONER EN FECHA 20 DE DICIEMBRE DE 2021 LAS PARTES QUE A CONTINUACIÓN SE DETALLAN.";
                string ParrafoTres = "LOS EQUIPOS/COMPONENTES DAÑADOS EL ADMINISTRADOR DEBERÁ IDENTIFICAR Y EMBALAR, ENVIANDOLOS EN UN PERÍODO DE 5 DÍAS MÁXIMO AL ÁLMACÉN DE LA COORDINACION REGIONAL POR OFICIO, PARA SU DESTINO FINAL CONFORME LA NORMA CAPUFE. ";
                string ParrafoCuatro = "LOS TRABAJOS, INSTALACIÓN Y OPERACIÓN DE LA PLAZA DE COBRO QUEDAN A ENTERA SATISFACCIÓN DEL ADMINISTRADOR DE LA PLAZA DE COBRO Y/O ENCARGADO DE TURNO.";

                var colParrafoUno = new PdfPCell(new Phrase(ParrafoUno, letraNormalMediana))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 15,
                    PaddingBottom = 10
                };

                var colParrafoDos = new PdfPCell(new Phrase(ParrafoDos, letraNormalMediana))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10
                };

                var colParrafoTres = new PdfPCell(new Phrase(ParrafoTres, letraNormalMediana))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10
                };

                var colParrafoCuatro = new PdfPCell(new Phrase(ParrafoCuatro, letraNormalMediana))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10
                };

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

        private IElement TablaComponentes()
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 14.28f, 14.28f, 14.28f, 14.28f, 14.28f, 14.28f, 14.28f }) { WidthPercentage = 100f };

                var encabezadoComponenteDañado = new PdfPCell(new Phrase("COMPONENTES Y/O REFACCIONES DAÑADAS:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 15, Colspan = 7 };

                var encabezadoCantidad = new PdfPCell(new Phrase("Cantidad", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 10 };
                var encabezadoComponente = new PdfPCell(new Phrase("Componente", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 10 };
                var encabezadoMarca = new PdfPCell(new Phrase("Marca Y/O Modelo", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 10 };
                var encabezadoSerie = new PdfPCell(new Phrase("No. de Serie", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 10 };
                var encabezadoInventario = new PdfPCell(new Phrase("No. Inventario", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 10 };
                var encabezadoUbicacion = new PdfPCell(new Phrase("Ubicacion", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 10 };
                var encabezadoObservaciones = new PdfPCell(new Phrase("Observaciones", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 10 };

                table.AddCell(encabezadoComponenteDañado);
                table.AddCell(encabezadoCantidad);
                table.AddCell(encabezadoComponente);
                table.AddCell(encabezadoMarca);
                table.AddCell(encabezadoSerie);
                table.AddCell(encabezadoInventario);
                table.AddCell(encabezadoUbicacion);
                table.AddCell(encabezadoObservaciones);

                CeldasContenido(7, table);

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

                var colParrafoUno = new PdfPCell(new Phrase(ParrafoUno, letraoNegritaGrande))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 15,
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

        private PdfPTable TablaFirmas()
        {
            try
            {
                PdfPTable tablaCuatroFirmas = new PdfPTable(new float[] { 22.5f, 3.333f, 22.5f, 3.333f, 22.5f, 3.333f, 22.5f }) { WidthPercentage = 100f };

                //Encabezado con firma 

                var capufe  = new PdfPCell(new Phrase("POR CAPUFE", letraoNegritaMediana)) 
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

                var empresa = new PdfPCell(new Phrase("POR LA EMPRESA", letraoNegritaMediana))
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

                var administrador = new PdfPCell(new Phrase("ADMINISTRADOR Y ENCARGADO DE LA SUPERINTENDENCIA DE OPERACIÓN", letraoNegritaMediana))
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

                var subgerente = new PdfPCell(new Phrase("SUBGERENTE DE OPERACIÓN CUERNAVACA", letraoNegritaMediana))
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

                //Nombres

                var nombreCapufe = new PdfPCell(new Phrase("LIC. JESÚS GERMAIN GONZÁLEZ MORALES.", letraoNegritaMediana))
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

                var nombreEmpresa = new PdfPCell(new Phrase("C. JHOVANY MOHAMED ISAURO GARCIA", letraoNegritaMediana))
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

                var nombreAdministrador = new PdfPCell(new Phrase("C. CARLOS BÁRCENAS ORTEGA SUPERVISÓN", letraoNegritaMediana))
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

                var nombreSubgerente = new PdfPCell(new Phrase("L.A.E. FIDEL URIBE HERNÁNDEZ", letraoNegritaMediana))
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

                //Indicativo preterito

                var nombresubgerente = new PdfPCell(new Phrase("LIC. JESÚS GERMAIN GONZÁLEZ MORALES.", letraoNegritaMediana))
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

                PdfPTable table = new PdfPTable(new float[] { 30f, 10f, 30f, 10f, 30f }) { WidthPercentage = 100f };
                table.TotalWidth = 550;
                CeldasVacias(15, table);
                var celdaVaciaFirmas = new PdfPCell() { Border = 0, FixedHeight = 30 };
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0, FixedHeight = 30 });
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0, FixedHeight = 30 });
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0, FixedHeight = 30 });
                //NOMRE Y FIRMA
                var colNombre = new PdfPCell(new Phrase("LIC. JESÚS GERMAIN GONZÁLEZ MORALES.", letraoNormalChicaFirmas))
                {
                    BorderWidth = 0,
                    BorderWidthTop = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 2
                };

                var colSello = new PdfPCell(new Phrase("C. JHOVANY MOHAMED ISAURO /n GARCIA", letraoNormalChicaFirmas))
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
                var colTecnico = new PdfPCell(new Phrase("Administrador o Encargado de Turno AUTORIZÓ", letraNormalChica))
                {   
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var colPersonal = new PdfPCell(new Phrase("Técnico REALIZÓ", letraNormalChica))
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
                return tablaCuatroFirmas;
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
