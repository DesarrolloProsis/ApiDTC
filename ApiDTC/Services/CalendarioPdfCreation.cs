

namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class CalendarioPdfCreation
    {
        #region Attributes

        private DataTable _tableHeader;

        private DataTable _tableActivities;

        private string _plaza;

        private ApiLogger _apiLogger;

        private int _month;

        private int _year;

        private string _square;
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
        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 11f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 5f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalGrande = new iTextSharp.text.Font(NormalGrande, 13f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 10f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraSubAzulChica = new iTextSharp.text.Font(NormalChicaSubAzul, 5f, iTextSharp.text.Font.UNDERLINE, BaseColor.Blue);
        public static iTextSharp.text.Font letritasMiniMini = new iTextSharp.text.Font(fuenteLetrita, 1f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 4f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion
        #region Logo
        #endregion
        #endregion

        #region Constructors
        public CalendarioPdfCreation(ApiLogger apiLogger, string plaza, int month, int year, string square)
        {
            _apiLogger = apiLogger;
            _plaza = plaza;
            _month = month;
            _year = year;
            _square = square;
        }

        public CalendarioPdfCreation(DataTable tableHeader, DataTable tableActivities, string plaza, ApiLogger apiLogger, int month, int year, string square)
        {
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _tableActivities = tableActivities;
            _plaza = plaza;
            _month = month;
            _year = year;
            _square = square;
        }

        #endregion

        #region Methods
        public Response NewPdf()
        {
            string directory, file;
            DateTime now = DateTime.Now;
            directory = $@"{System.Environment.CurrentDirectory}\Bitacora\CalendariosMantenimiento\{_plaza}\{now.Year}\{MesActual()}\{now.Day}";
            file = $@"{directory}\{_plaza}{now.Year}{MesContrato(now)}C.pdf";
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
                            Message = $@"Error: Archivo {directory}\{_plaza}{now.Year}{MesContrato(now)}C.pdf en uso o inaccesible",
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
                    doc.SetMargins(30.8661f, 30.8661f, 30f, 28.3465f);
                    doc.AddAuthor("PROSIS");
                    doc.AddTitle("Calendario de mantenimiento preventivo");

                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelper();
                    writer.Open();

                    doc.Open();

                    doc.Add(TablaEncabezado());
                    doc.Add(TablaFechas());
                    doc.Add(TablaObservaciones());
                    doc.Add(new Paragraph(" "));
                    doc.Add(TablaFirmas());



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
                if (System.IO.File.Exists($@"{System.Environment.CurrentDirectory}\CalendariosMantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\{_plaza}{DateTime.Now.Year}01C.pdf"))
                    System.IO.File.Delete($@"{System.Environment.CurrentDirectory}\CalendariosMantenimiento\{DateTime.Now.Year}\{MesActual()}\{DateTime.Now.Day}\{_plaza}{DateTime.Now.Year}01C.pdf");
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
        
        private string MesContrato(DateTime fechaSolicitud)
        {
            DateTime contratoInicial = new DateTime(2020, 11, 1);
            int mesesTranscurridos = (contratoInicial.Month - fechaSolicitud.Month) + (12 * (contratoInicial.Year - fechaSolicitud.Year)) + 1;
            return mesesTranscurridos.ToString("00");
        }
        
        private IElement TablaEncabezado()
        {
            try
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\prosis-logo.jpg");
                logo.ScalePercent(10f);

                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 10, 30f, 20f, 30f, 10f }) { WidthPercentage = 100f };

                var celdaVacia = new PdfPCell() { Border = 0 };
                PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 3 };           
                table.AddCell(colLogo);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                var celdaSalto = new PdfPCell() { Colspan = 5, Border = 0 };
                table.AddCell(celdaSalto);

                var colTitulo = new PdfPCell(new Phrase("CALENDARIO DE MANTENIMIENTO PREVENTIVO", letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 3 };
                table.AddCell(celdaVacia);
                table.AddCell(colTitulo);
                table.AddCell(celdaVacia);

                var mesCalendarioPreventivo = new Chunk($"CORRESPONDIENTE AL MES DE: {MesActual(_month).ToUpper()} DEL {_year}", letraoNegritaMediana);
                var phraseMes = new Phrase(mesCalendarioPreventivo);
                var colMes = new PdfPCell(phraseMes) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2, Colspan = 2 };
                table.AddCell(celdaVacia);
                table.AddCell(colMes);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                string plaza;
                if (_tableHeader != null)
                {
                    if (_tableHeader.Rows.Count > 0)
                        plaza = _tableHeader.Rows[0]["SquareName"].ToString();
                    else
                        plaza = "";
                }
                else
                    plaza = "";

                var plazaDeCobro = new Chunk($"   PLAZA DE COBRO: {_square} {plaza}", letraoNegritaMediana);
                var phraseCobro = new Phrase(plazaDeCobro);
                var colCobro = new PdfPCell(phraseCobro) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                table.AddCell(celdaVacia);
                table.AddCell(colCobro);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(ex, $"TablaEncabezado error: {ex.Message}");
                return null;
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(ex, $"TablaEncabezado error: {ex.Message}");
                return null;
            }
            
        }

        
        private IElement TablaFechas()
        {
            
            PdfPTable table = new PdfPTable(new float[] { 14.3f, 14.30f, 14.3f, 14.3f, 14.3f, 14.3f, 14.3f }) { WidthPercentage = 100f };
            var celdaVacia = new PdfPCell() { Border = 0 };
            var colTitulo = new PdfPCell(new Phrase("FECHAS PROPUESTAS", letraoNegritaGrande)) { Border = 0,  HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingRight = 20, PaddingLeft = 20, Colspan = 5 };
            table.AddCell(celdaVacia);
            table.AddCell(colTitulo);
            table.AddCell(celdaVacia);

            int days = DateTime.DaysInMonth(_year, _month);
            

            int totalCeldas = 35;
            int carriles = 0;

            //Revisión seis semanas
            DateTime diaInicial = new DateTime(_year, _month, 1);
            bool mesSeisSemanas = false;
            if (string.Equals(DiaActual(diaInicial).ToLower(), "sábado") && (days == 30 || days == 31))
                totalCeldas += 7;
            else if (string.Equals(DiaActual(diaInicial).ToLower(), "viernes") && days == 31)
                totalCeldas += 7;

            int recorridoCalendario = RecorridoCeldasCalendario(DiaActual(new DateTime(_year, _month, 1)));

            for (int i = 0; i < recorridoCalendario; i++)
            {
                table.AddCell(new PdfPCell(new Phrase(" ", letraNormalChica)) { BackgroundColor = BaseColor.LightGray, BorderWidth = 1, FixedHeight = 10, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
            }
            //totalCeldas -= recorridoCalendario;
            List<ActivitiesSql> lanes = new List<ActivitiesSql>();
            foreach (DataRow row in _tableActivities.Rows)
            {
                lanes.Add(new ActivitiesSql
                {
                    CapufeLaneNum = row["CapufeLaneNum"].ToString(),
                    Lane = row["Lane"].ToString(),
                    IdGare = row["IdGare"].ToString(),
                    Day = Convert.ToInt32(row["Day"])
                });
            }

            bool primerRecorrido = false;

            for (int i = 0; i < totalCeldas; i++)
            {
                PdfPCell celdaFecha;

                if((i + 1) > days)
                {
                    celdaFecha = new PdfPCell(new Phrase(" ", letraNormalChica)) { BackgroundColor = BaseColor.LightGray, BorderWidth = 1, FixedHeight = 10, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                }
                else
                {
                    string fecha = $"{DiaActual(new DateTime(_year, _month, i + 1))} {i + 1} {MesActual(_month)}";
                    celdaFecha = new PdfPCell(new Phrase(fecha, letraNormalChica)) { BackgroundColor = BaseColor.LightGray, BorderWidth = 1, FixedHeight = 10, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                }
                
                table.AddCell(celdaFecha);

                if ((i + 1 + recorridoCalendario) % 7 == 0)
                {
                    int numeroCeldasActividades = primerRecorrido ? 7 : 7 - recorridoCalendario;

                    if (!primerRecorrido)
                    {
                        for (int j = 0; j < recorridoCalendario; j++)
                        {
                            var celdaContenido = new PdfPCell(new Phrase("", letraNormalChica)) { BorderWidth = 1, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                            table.AddCell(celdaContenido);
                        }
                        primerRecorrido = true;
                    }
                    
                    for (int j = 0; j < numeroCeldasActividades; j++)
                    {
                        carriles++;
                        string descripcion = "";
                        var items = lanes.Where(x => x.Day == carriles);
                        foreach (var item in items)
                        {
                            descripcion += item.Lane + " ";
                        }
                        var stringCarriles = descripcion.Split(' ').OrderBy(x => x);
                        var celdaContenido = new PdfPCell(new Phrase(string.Join(' ', stringCarriles), letraNormalChica)) { BorderWidth = 1, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                        table.AddCell(celdaContenido);
                    }
                }
                
            };

            return table;
        }

        private IElement TablaObservaciones()
        {

            PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };
            
            var celdaVacia = new PdfPCell() { Border = 0 };
            table.AddCell(celdaVacia);
            table.AddCell(celdaVacia);

            var colTitulo = new PdfPCell(new Phrase("OBSERVACIONES", new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black))) 
            { 
                BorderWidth = 1, 
                HorizontalAlignment = Element.ALIGN_CENTER, 
                VerticalAlignment = Element.ALIGN_CENTER, 
                Padding = 5
            };

            string comentario;
            if (_tableHeader != null)
            {
                if (_tableHeader.Rows.Count > 0)
                    comentario = _tableHeader.Rows[0]["Comment"].ToString();
                else
                    comentario = "";
            }
            else
                comentario = "";

            var celdaObservaciones = new PdfPCell(new Phrase(comentario, new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black))) 

            //var celdaObservaciones = new PdfPCell(new Phrase("What is Lorem Ipsum ?Lorem Ipsum is simply dummy text of the printing and typesetting industry.Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", new iTextSharp.text.Font(NormalChica, 7f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
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

        private IElement TablaFirmas()
        {

            PdfPTable table = new PdfPTable(new float[] { 40f, 20f, 40f }) { WidthPercentage = 100f };


            var celdaVacia = new PdfPCell() { Border = 0 };
            string userName, adminName;

            if (_tableHeader != null)
            {
                if (_tableHeader.Rows.Count > 0)
                    userName = _tableHeader.Rows[0]["UserName"].ToString();
                else
                    userName = "";
            }
            else
                userName = "";

            var colNombreTecnico = new PdfPCell(new Phrase(userName, new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
            {
                BorderWidth = 0,
                BorderWidthBottom = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };

            if (_tableHeader != null)
            {
                if (_tableHeader.Rows.Count > 0)
                    adminName = _tableHeader.Rows[0]["AdminName"].ToString();
                else
                    adminName = "";
            }
            else
                adminName = "";

            var colNombreEncargado = new PdfPCell(new Phrase(adminName, new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black)))
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

        private string DiaActual(DateTime day) { return new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat.GetDayName(day.DayOfWeek); }

        private string MesActual(int month) { return new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(month); }

        private int RecorridoCeldasCalendario(string diaInicial)
        {
            switch (diaInicial.ToLower())
            {
                case "domingo":
                    return 0;
                case "lunes":
                    return 1;
                case "martes":
                    return 2;
                case "miércoles":
                    return 3;
                case "jueves":
                    return 4;
                case "viernes":
                    return 5;
                case "sábado":
                    return 6;
                default:
                    return 0;
            }
        }
        #endregion
    }
}
