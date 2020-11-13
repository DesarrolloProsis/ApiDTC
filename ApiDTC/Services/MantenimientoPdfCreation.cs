

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

    public class MantenimientoPdfCreation
    {
        #region Attributes

        private DataTable _tableHeader;

        private DataTable _tableActivities;

        private string _folio;

        private ApiLogger _apiLogger;

        private int _tipo;
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
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 11f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
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
        public MantenimientoPdfCreation(ApiLogger apiLogger, string folio, int tipo)
        {
            _apiLogger = apiLogger;
            _folio = folio;
            _tipo = tipo;
        }

        public MantenimientoPdfCreation(DataTable tableHeader, DataTable tableActivities, string folio, ApiLogger apiLogger, int tipo)
        {
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _tableActivities = tableActivities;
            _folio = folio;
            _tipo = tipo;
        }

        #endregion

        #region Methods
        public Response NewPdf()
        {
            string directory, file;
            directory = $@"{System.Environment.CurrentDirectory}\Mantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\";
            file = $@"{System.Environment.CurrentDirectory}\Mantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\Mantenimiento-{_folio}.pdf";
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
                            Message = $"Error: Archivo Mantenimiento-{_folio} en uso o inaccesible",
                            Result = null
                        };
                    }
                }
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(ex, "MantenimientoPdfCreation");
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
                    doc.SetMargins(35, 35, 60.8661f, 60.8661f);
                    doc.AddAuthor("PROSIS");
                    switch (_tipo)
                    {
                        case 1: 
                            doc.AddTitle("Mantenimiento semanal preventivo");  
                            break;
                        case 2:
                            doc.AddTitle("Mantenimiento mensual preventivo");
                            break;
                        case 3:
                            doc.AddTitle("Mantenimiento bimestral preventivo");
                            break;
                        case 4:
                            doc.AddTitle("Mantenimiento semestral preventivo");
                            break;
                        case 5:
                            doc.AddTitle("Mantenimiento anual preventivo");
                            break;
                        default: break;
                    }
                    

                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelper();
                    writer.Open();

                    doc.Open();

                    doc.Add(tablaEncabezado());
                    doc.Add(new Phrase(" "));
                    doc.Add(tablaInformacion());
                    doc.Add(tablaDescripcion());
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
            PdfPTable table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };

            var celdaVacia = new PdfPCell() { Border = 0 };
            PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
            table.AddCell(colLogo);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var celdaSalto = new PdfPCell() { Colspan = 5, Border = 0 };
            table.AddCell(celdaSalto);
            string textoTitulo = "";
            switch (_tipo)
            {
                case 1:
                    textoTitulo = "Mantenimiento semanal preventivo";
                    break;
                case 2:
                    textoTitulo = "Mantenimiento mensual preventivo";
                    break;
                case 3:
                    textoTitulo = "Mantenimiento bimestral preventivo";
                    break;
                case 4:
                    textoTitulo = "Mantenimiento semestral preventivo";
                    break;
                case 5:
                    textoTitulo = "Mantenimiento anual preventivo";
                    break;
                default: break;
            }
            var colTitulo = new PdfPCell(new Phrase(textoTitulo, letraNormalMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 2 };
            table.AddCell(celdaVacia);
            table.AddCell(colTitulo);
            table.AddCell(celdaVacia);

            return table;
        }

        private IElement tablaInformacion()
        {
            
            PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
            var celdaVacia = new PdfPCell() { Border = 0 };

            var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte: ", letraoNegritaChica)) { Border = 0,  HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
            var colNoReporte = new PdfPCell(new Phrase("TEP-072020JUL", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
            var colTextoFecha = new PdfPCell(new Phrase("Fecha: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
            var colFecha = new PdfPCell(new Phrase("15/07/2020", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

            table.AddCell(celdaVacia);
            table.AddCell(colTextoNoReporte);
            table.AddCell(colNoReporte);
            table.AddCell(celdaVacia);
            table.AddCell(colTextoFecha);
            table.AddCell(colFecha);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);


            var colTextoHoraInicio = new PdfPCell(new Phrase("Hora inicio: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
            var colHoraInicio = new PdfPCell(new Phrase("11:15", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(colTextoHoraInicio);
            table.AddCell(colHoraInicio);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var colTextoHoraFin = new PdfPCell(new Phrase("Hora fin: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
            var colHoraFin = new PdfPCell(new Phrase("12:20", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(colTextoHoraFin);
            table.AddCell(colHoraFin);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var colTextoPlazaDeCobro = new PdfPCell(new Phrase("Plaza de cobro: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
            var colPlazaDeCobro = new PdfPCell(new Phrase("4 TEPOTZOTLAN", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 , Colspan = 3};

            table.AddCell(celdaVacia);
            table.AddCell(colTextoPlazaDeCobro);
            table.AddCell(colPlazaDeCobro);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var colTextoPersonaProsis = new PdfPCell(new Phrase("Persona de PROSIS que realiza: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 2 };
            var colPersonaProsis = new PdfPCell(new Phrase("ALEJANDRO DE LA ROSA VILLANUEVA", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2};

            table.AddCell(celdaVacia);
            table.AddCell(colTextoPersonaProsis);
            table.AddCell(colPersonaProsis);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var colTextoPersonaCapufe = new PdfPCell(new Phrase("Persona de CAPUFE que recibe: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 2};
            var colPersonaCapufe = new PdfPCell(new Phrase("LIC RAFAEL CASTREJON SALAZAR", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };

            table.AddCell(celdaVacia);
            table.AddCell(colTextoPersonaCapufe);
            table.AddCell(colPersonaCapufe);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            
            var colTextoCarril = new PdfPCell(new Phrase("Carril: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };
            var colCarril = new PdfPCell(new Phrase("TEP-B06", letraNormalChica)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
            var colTipoCarril = new PdfPCell(new Phrase("MULTIMODAL ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

            table.AddCell(celdaVacia);
            table.AddCell(colTextoCarril);
            table.AddCell(colCarril);
            table.AddCell(colTipoCarril);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            return table;
        }

        private IElement tablaDescripcion()
        {

            PdfPTable table = new PdfPTable(new float[] { 20f, 15f, 15f, 15f, 15f, 20f}) { WidthPercentage = 100f };
            var celdaVacia = new PdfPCell() { Border = 0 };
            //ESPACIO
            for (int i = 0; i < 12; i++)
            {
                table.AddCell(celdaVacia);
            }
            

            var colEquipoCarril = new PdfPCell(new Phrase("Equipo de Carril", letraoNegritaMediana)) 
            { 
                BorderWidth = 1, 
                HorizontalAlignment = Element.ALIGN_CENTER, 
                VerticalAlignment = Element.ALIGN_CENTER, 
                Padding = 3,
                Colspan = 2
            };
            var colDescripcionActividad = new PdfPCell(new Phrase("OK - N/A", letraoNegritaMediana))
            {
                BorderWidth = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 3,
                Colspan = 1
            };

            table.AddCell(celdaVacia);
            table.AddCell(colEquipoCarril);
            table.AddCell(colDescripcionActividad);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            for (int i = 0; i < 32; i++)
            {
                var colEquipo = new PdfPCell(new Phrase("Semáforo de techo modo de pago", letraNormalChica))
                {
                    BorderWidth = 1,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 2,
                    Colspan = 2
                };
                var colDescripcion = new PdfPCell(new Phrase("Sí", letraNormalChica))
                {
                    BorderWidth = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 2,
                    Colspan = 1
                };
                table.AddCell(celdaVacia);
                table.AddCell(colEquipo);
                table.AddCell(colDescripcion);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
            }

            return table;
        }

        private IElement tablaObservaciones()
        {

            PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
            var celdaVacia = new PdfPCell() { Border = 0 };

            //ESPACIO
            for (int i = 0; i < 16; i++)
            {
                table.AddCell(celdaVacia);
            }

            var colTextoMotivo = new PdfPCell(new Phrase("Motivo:", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };

            table.AddCell(celdaVacia);
            table.AddCell(colTextoMotivo);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            for (int i = 0; i < 16; i++)
            {
                table.AddCell(celdaVacia);
            }


            var colTextoModulo = new PdfPCell(new Phrase("Sin acceso a Módulo de Fallas: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 1 };
            var colBoolModulo = new PdfPCell(new Phrase("X", letraNormalChica)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
            var colTextoSinConexion = new PdfPCell(new Phrase("Sin conexión: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 1 };
            var colBoolSinConexion = new PdfPCell(new Phrase("X", letraNormalChica)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
            var colTextoOtros = new PdfPCell(new Phrase("Otros: ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 1 };
            var colBoolOtros = new PdfPCell(new Phrase("X", letraNormalChica)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };

            table.AddCell(celdaVacia);
            table.AddCell(colTextoModulo);
            table.AddCell(colBoolModulo);
            table.AddCell(colTextoSinConexion);
            table.AddCell(colBoolSinConexion);
            table.AddCell(colTextoOtros);
            table.AddCell(colBoolOtros);
            table.AddCell(celdaVacia);


            var colTextoObservaciones = new PdfPCell(new Phrase("Observaciones: ", letraoNegritaChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };

            table.AddCell(celdaVacia);
            table.AddCell(colTextoObservaciones);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var celdaObservaciones = new PdfPCell(new Phrase("What is Lorem Ipsum ? Lorem Ipsum is simply dummy text of the printing and typesetting industry.Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", letraNormalChica)) { BorderWidth = 0, VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_JUSTIFIED, Colspan = 6 };

            table.AddCell(celdaVacia);
            table.AddCell(celdaObservaciones);
            table.AddCell(celdaVacia);

            return table;
        }

        private IElement tablaFirmas()
        {

            PdfPTable table = new PdfPTable(new float[] { 40f, 20f, 40f }) { WidthPercentage = 100f };


            var celdaVacia = new PdfPCell() { Border = 0 };
            var colNombreTecnico = new PdfPCell(new Phrase("ALEJANDRO DE LA ROSA VILLANUEVA", new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
            {
                BorderWidth = 0,
                BorderWidthBottom = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            var colNombreEncargado = new PdfPCell(new Phrase("LIC RAFAEL CASTREJON SALAZAR", new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
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

            var colTecnico = new PdfPCell(new Phrase("PROSIS", new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
            {
                BorderWidth = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            var colEncargado = new PdfPCell(new Phrase("Personal plaza de cobro", new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
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
