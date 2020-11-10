

namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class CalendarioPdfCreation
    {
        #region Attributes

        private DataTable _tableHeader;

        private DataTable _tableActivities;

        private string _folio;

        private ApiLogger _apiLogger;
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
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 5f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalGrande = new iTextSharp.text.Font(NormalGrande, 15f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 11f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraSubAzulChica = new iTextSharp.text.Font(NormalChicaSubAzul, 5f, iTextSharp.text.Font.UNDERLINE, BaseColor.Blue);
        public static iTextSharp.text.Font letritasMiniMini = new iTextSharp.text.Font(fuenteLetrita, 1f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion
        #region Logo
        #endregion
        #endregion

        #region Constructors
        public CalendarioPdfCreation(ApiLogger apiLogger, string folio)
        {
            _apiLogger = apiLogger;
            _folio = folio;
        }

        public CalendarioPdfCreation(DataTable tableHeader, DataTable tableActivities, string folio, ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _tableActivities = tableActivities;
            _folio = folio;
        }

        #endregion

        #region Methods
        public Response NewPdf()
        {
            string directory, file;
            directory = $@"{System.Environment.CurrentDirectory}\CalendariosMantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\";
            file = $@"{System.Environment.CurrentDirectory}\CalendariosMantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\CalendarioMantenimiento-{_folio}.pdf";
            //If file exists
            try
            {   
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                if (File.Exists(file))
                {
                    if (FileInUse(file))
                    {
                        return new Response
                        {
                            Message = $"Error: Archivo CalendarioMantenimiento-{_folio} en uso o inaccesible",
                            Result = null
                        };
                    }
                }
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(ex, "CalendarioPdfCreation");
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
                    doc.SetPageSize(new Rectangle(793.701f, 609.4488f));
                    doc.SetMargins(70.8661f, 70.8661f, 40f, 28.3465f);
                    doc.AddAuthor("PROSIS");
                    doc.AddTitle("Calendario de mantenimiento preventivo");

                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelper();
                    writer.Open();

                    doc.Open();

                    doc.Add(tablaEncabezado());
                    doc.Add(new Phrase(" "));
                    doc.Add(tablaFechas());
                    doc.Add(new Phrase(" "));
                    doc.Add(tablaObservaciones());
                    doc.Add(new Phrase(" "));
                    doc.Add(new Phrase(" "));
                    doc.Add(new Phrase(" "));
                    doc.Add(tablaFirmas());



                    doc.Close();
                    writer.Close();
                    byte[] content = myMemoryStream.ToArray();


                    using (FileStream fs = File.Create(file))
                    {
                        fs.Write(content, 0, (int)content.Length);
                    }
                }
            }
            catch (IOException ex)
            {
                if (System.IO.File.Exists($@"{System.Environment.CurrentDirectory}\CalendariosMantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\CalendarioMantenimiento-{_folio}.pdf"))
                    System.IO.File.Delete($@"{System.Environment.CurrentDirectory}\CalendariosMantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\CalendarioMantenimiento-{_folio}.pdf");
                _apiLogger.WriteLog(ex, "CalendarioPdfCreation");
                return new Response
                {
                    Message = $"Error: {ex.Message}.",
                    Result = null
                };
            }
            return new Response
            {
                Message = "Ok",
                Result = file
            };
        }

        private IElement tablaEncabezado()
        {
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\prosis-logo.jpg");
            logo.ScalePercent(10f);

            //Encabezado
            PdfPTable table = new PdfPTable(new float[] { 10, 30f, 20f, 30f, 10f }) { WidthPercentage = 100f };

            var celdaVacia = new PdfPCell() { Border = 0 };
            PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 };
            table.AddCell(celdaVacia);
            table.AddCell(colLogo);
            table.AddCell(celdaVacia);

            var celdaSalto = new PdfPCell() { Colspan = 5, Border = 0 };
            table.AddCell(celdaSalto);

            var colTitulo = new PdfPCell(new Phrase("CALENDARIO DE MANTENIMIENTO PREVENTIVO", letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 3 };
            table.AddCell(celdaVacia);
            table.AddCell(colTitulo);
            table.AddCell(celdaVacia);

            var mesCalendarioPreventivo = new Chunk("CORRESPONDIENTE AL MES DE: JULIO DEL 2020", letraoNegritaMediana);
            var phraseMes = new Phrase(mesCalendarioPreventivo);
            var colMes = new PdfPCell(phraseMes) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2};
            table.AddCell(celdaVacia);
            table.AddCell(colMes);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var plazaDeCobro = new Chunk("   PLAZA DE COBRO:  PALMILLAS" , letraoNegritaMediana);
            var phraseCobro = new Phrase(plazaDeCobro);
            var colCobro = new PdfPCell(phraseCobro) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
            table.AddCell(celdaVacia);
            table.AddCell(colCobro);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            return table;
        }

        private IElement tablaFechas()
        {
            
            PdfPTable table = new PdfPTable(new float[] { 20, 20f, 20f, 20f, 20f }) { WidthPercentage = 100f };
            var celdaVacia = new PdfPCell() { Border = 0 };
            var colTitulo = new PdfPCell(new Phrase("FECHAS PROPUESTAS", letraoNegritaGrande)) { Border = 0,  HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 3 };
            table.AddCell(celdaVacia);
            table.AddCell(colTitulo);
            table.AddCell(celdaVacia);

            var celdaFecha = new PdfPCell(new Phrase("LUNES 29 JUNIO", letraNormalChica)) { BackgroundColor = BaseColor.LightGray, BorderWidth = 1, FixedHeight = 15, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, Rowspan = 2};
            var celdaContenido = new PdfPCell(new Phrase("PLAZA", letraNormalChica)) { BorderWidth = 1, FixedHeight = 15, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, Rowspan = 2 };

            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);

            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);

            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);

            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);

            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);

            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);

            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);
            table.AddCell(celdaFecha);

            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);
            table.AddCell(celdaContenido);

            return table;
        }

        private IElement tablaObservaciones()
        {

            PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };

            var colTitulo = new PdfPCell(new Phrase("OBSERVACIONES", new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black))) 
            { 
                BorderWidth = 1, 
                HorizontalAlignment = Element.ALIGN_CENTER, 
                VerticalAlignment = Element.ALIGN_CENTER, 
                Padding = 5
            };

            var celdaObservaciones = new PdfPCell(new Phrase("MANTENIMIENTO A SERVIDOR DE PLAZA, SERVIDORES DE VIDEO, KVM, RUTEADOR, SWITCHES DE COMUNICACIONES, CENTRAL TELEFÓNICA, CONVERTIDORES DE FIBRA ÓPTICA, ESTACIONES DE TRABAJO EN PLAZA DE COBRO, IMPRESORAS DE REPORTES Y TELEFONOS IP EN PLAZA DE COBRO.", new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black))) 
            { 
                VerticalAlignment = Element.ALIGN_MIDDLE, 
                HorizontalAlignment = Element.ALIGN_JUSTIFIED, 
                BorderWidth = 1,
                Padding = 5
            };

            table.AddCell(colTitulo);
            table.AddCell(celdaObservaciones);

            return table;
        }

        private IElement tablaFirmas()
        {

            PdfPTable table = new PdfPTable(new float[] { 40f, 20f, 40f }) { WidthPercentage = 100f };


            var celdaVacia = new PdfPCell() { Border = 0 };
            var colNombreTecnico = new PdfPCell(new Phrase("NOÉ FERNANDO ORTEGA VAZQUEZ", new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
            {
                BorderWidth = 0,
                BorderWidthBottom = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            var colNombreEncargado = new PdfPCell(new Phrase("C.P. GUSTAVO GASPAR RODRIGUEZ", new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
            {
                BorderWidth = 0,
                BorderWidthBottom = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(colNombreTecnico);
            table.AddCell(celdaVacia);
            table.AddCell(colNombreEncargado);

            var colTecnico = new PdfPCell(new Phrase("TÉCNICO PROSIS", new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
            {
                BorderWidth = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            var colEncargado = new PdfPCell(new Phrase("NOMBRE FIRMA Y SELLO DE PLAZA DE COBRO", new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
            {
                BorderWidth = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(colTecnico);
            table.AddCell(celdaVacia);
            table.AddCell(colEncargado);

            return table;
        }

        private bool FileInUse(string file)
        {
            bool fileInUse = false;
            try
            {
                FileStream fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(ex, "FileInUse");
                fileInUse = true;
            }
            return fileInUse;
        }

        private string MesActual() { return new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(DateTime.Now.Month); }
        #endregion
    }
}
