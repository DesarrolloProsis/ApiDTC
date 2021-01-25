

namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    public class MantenimientoPdfCreation
    {
        #region Attributes

        private readonly DataTable _tableHeader;

        private readonly DataTable _tableActivities;

        private readonly string _clavePlaza;

        private readonly ApiLogger _apiLogger;

        private readonly int _tipo;

        private readonly string[] _temporal;

        private readonly string _ubicacion;

        private readonly string _noReporte;

        private string _textoTitulo;
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
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 7f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalGrande = new iTextSharp.text.Font(NormalGrande, 15f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
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

        public MantenimientoPdfCreation(string clavePlaza, DataTable tableHeader, DataTable tableActivities, ApiLogger apiLogger, int tipo, string ubicacion, string noReporte)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _tableActivities = tableActivities;
            _tipo = tipo;
            _temporal = TipoDeReporte(_tipo);
            _ubicacion = ubicacion;
            _noReporte = noReporte;
        }

        public MantenimientoPdfCreation(string clavePlaza, ApiLogger apiLogger, int tipo, string ubicacion, string noReporte)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tipo = tipo;
            _temporal = TipoDeReporte(_tipo);
            _ubicacion = ubicacion;
            _noReporte = noReporte;
        }

        #endregion
        //https://localhost:44358/api/MantenimientoPdf/Nuevo/Jor/jorobas/B01/TLA-20333
        #region Methods
        public Response NewPdf()
        {
            DateTime now = DateTime.Now;
            string directory = $@"C:\Bitacora\{_clavePlaza}\Mantenimiento\{_noReporte}";
            string filename = $"{_clavePlaza.ToUpper()}{DateTime.Now.Year}{MesContrato(now)}{_ubicacion}{_temporal[0]}.pdf";
            
            string path = Path.Combine(directory, filename);

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
                            Message = $"Error: Archivo {filename} en uso o inaccesible",
                            Result = null
                        };
                    }
                    File.Delete(path);
                }
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MantenimientoPdfCreation: NewPdf", 2);
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
                    doc.SetMargins(30, 30, 30, 30);
                    doc.AddAuthor("PROSIS");
                    switch (_tipo)
                    {
                        case 1: 
                            doc.AddTitle("Mantenimiento preventivo semanal nivel plaza");  
                            break;
                        case 2:
                            doc.AddTitle("Mantenimiento preventivo mensual nivel plaza");
                            break;
                        case 3:
                            doc.AddTitle("Mantenimiento preventivo trimestral nivel plaza");
                            break;
                        case 4:
                            doc.AddTitle("Mantenimiento preventivo semestral nivel plaza");
                            break;
                        case 5:
                            doc.AddTitle("Mantenimiento preventivo anual nivel plaza");
                            break;
                        case 6:
                            doc.AddTitle("Mantenimiento preventivo semanal nivel carril");
                            break;
                        case 7:
                            doc.AddTitle("Mantenimiento preventivo mensual nivel carril");
                            break;
                        case 8:
                            doc.AddTitle("Mantenimiento preventivo trimestral nivel carril");
                            break;
                        case 9:
                            doc.AddTitle("Mantenimiento preventivo semestral nivel carril");
                            break;
                        case 10:
                            doc.AddTitle("Mantenimiento preventivo anual nivel carril");
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
                    doc.Add(TablaDescripcion());
                    doc.Add(TablaObservaciones());
                    doc.Add(new Phrase(" "));
                    doc.Add(new Phrase(" "));
                    doc.Add(new Phrase(" "));

                    //Pdf fotografías evidencia
                    string directoryImgs = $@"{System.Environment.CurrentDirectory}\Reportes\2020\septiembre\24\PruebaMantenimiento\";
                    //string directorioEvidencias = $@"C:\Bitacora\{_clavePlaza}\Mantenimiento\{_noReporte}\EvidenciasFotograficas";
                    var fotos = Directory.GetFiles(directoryImgs);
                    if(fotos.Length != 0)
                    {
                        doc.NewPage();

                        int paginasNecesarias = fotos.Length / 9 + (fotos.Length % 9 != 0 ? 1 : 0);

                        for (int i = 0; i < paginasNecesarias; i++)
                        {
                            if (i == 0)
                            {
                                doc.Add(TablaEncabezadoEvidencias(i + 1));
                                doc.Add(TablaFotografias(fotos, i + 1, paginasNecesarias));
                                continue;
                            }
                            else
                                doc.NewPage();
                            doc.Add(TablaEncabezadoEvidencias(i + 1));
                            doc.Add(TablaFotografias(fotos, i + 1, paginasNecesarias));

                           // if (i == paginasNecesarias - 1)
                                
                        }
                        doc.Add(TablaFirmas());
                    }
                   
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
                if (File.Exists(path))
                    File.Delete(path);
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: NewPdf", 2);
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

        private string MesContrato(DateTime fechaSolicitud)
        {
            try
            {
                DateTime contratoInicial = new DateTime(2020, 11, 1);
                int mesesTranscurridos = (contratoInicial.Month - fechaSolicitud.Month) + (12 * (contratoInicial.Year - fechaSolicitud.Year)) + 1;
                return mesesTranscurridos.ToString("00");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: NewPdf", 2);
                return null;
            }
            
        }

        private IElement TablaEncabezado()
        {
            try
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\prosis-logo.jpg");
                logo.ScalePercent(10f);

                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };

                var celdaVacia = new PdfPCell() { Border = 0 };
                
                PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                table.AddCell(colLogo);
                table.AddCell(celdaVacia);

                var celdaSalto = new PdfPCell() { Colspan = 5, Border = 0 };
                table.AddCell(celdaSalto);
                switch (_tipo)
                {
                    case 1:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO SEMANAL NIVEL PLAZA";
                        break;
                    case 2:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO MENSUAL NIVEL PLAZA";
                        break;
                    case 3:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO TRIMESTRAL NIVEL PLAZA";
                        break;
                    case 4:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO SEMESTRAL NIVEL PLAZA";
                        break;
                    case 5:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO ANUAL NIVEL PLAZA";
                        break;
                    case 6:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO SEMANAL NIVEL CARRIL";
                        break;
                    case 7:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO MENSUAL NIVEL CARRIL";
                        break;
                    case 8:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO TRIMESTRAL NIVEL CARRIL";
                        break;
                    case 9:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO SEMESTRAL NIVEL CARRIL";
                        break;
                    case 10:
                        _textoTitulo = "MANTENIMIENTO PREVENTIVO ANUAL NIVEL CARRIL";
                        break;
                    default: break;
                }
                var colTitulo = new PdfPCell(new Phrase(_textoTitulo, letraNormalMediana)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 4, Border = 0 };
                table.AddCell(colTitulo);

                return table;

            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaEncabezado", 5);
                return null;
            }
            catch(Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaEncabezado", 3);
                return null;
            }
            
        }

        private IElement TablaEncabezadoEvidencias(int pagina)
        {
            try
            {
                Image logo = Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\prosis-logo.jpg");
                logo.ScalePercent(10f);
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };

                var celdaVacia = new PdfPCell() { Border = 0 };
                PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                table.AddCell(colLogo);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);


                var celdaSalto = new PdfPCell() { Colspan = 4, Border = 0 };

                var colTitulo = new PdfPCell(new Phrase(_textoTitulo, letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 2 };
                table.AddCell(celdaVacia);
                table.AddCell(colTitulo); ;
                table.AddCell(celdaVacia);


                var colPlaza = new PdfPCell(new Phrase("PLAZA (PONER PALMILLAS)", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 2 };
                table.AddCell(celdaVacia);
                table.AddCell(colPlaza); ;
                table.AddCell(celdaVacia);


                var colReporte = new PdfPCell(new Phrase($"REPORTE: (pág. {pagina})", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 2 };
                table.AddCell(celdaVacia);
                table.AddCell(colReporte);
                table.AddCell(celdaVacia);

                table.AddCell(celdaSalto);

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaEncabezadoEvidencias", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaEncabezadoEvidencias", 3);
                return null;
            }
        }

        private IElement TablaFotografias(string[] rutas, int indice, int ultimo)
        {

            try
            {
                PdfPTable table = new PdfPTable(new float[] { 33f, 33f, 33f }) { WidthPercentage = 100f };
                var celdaVacia = new PdfPCell() { Border = 0, FixedHeight = 150 };

                int inicio, hasta;

                //Inicio del recorrido
                if (indice == 1)
                    inicio = 0;
                else
                    inicio = ((indice - 1) * 9);

                //Hasta donde
                if (rutas.Length % 9 == 0)
                    hasta = (indice * 9);
                else if (rutas.Length < 9)
                    hasta = rutas.Length % 9;
                else if (inicio == 0 && rutas.Length > 9)
                    hasta = 9;
                else if (indice == ultimo)
                    hasta = inicio + (rutas.Length % 9);
                else
                    hasta = inicio + 9;

                for (int i = inicio; i < hasta; i++)
                {
                    Image img = Image.GetInstance(rutas[i]);
                    if (img.Width > img.Height)
                        img.ScaleAbsolute(170f, 150f);
                    else
                        img.ScaleAbsolute(150f, 170f);
                    PdfPCell colFoto = new PdfPCell(img) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                    table.AddCell(colFoto);
                }

                //for (int i = 0; i < (9 - fotos.Count) + 3; i++)
                for (int i = 0; i < 9 - (hasta - inicio); i++)
                    table.AddCell(celdaVacia);

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaFotografias", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaFotografias", 3);
                return null;
            }
        }

        private IElement TablaInformacion()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                var celdaVacia = new PdfPCell() { Border = 0 };

                var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };
                var colNoReporte = new PdfPCell(new Phrase("TLA-MPA-01", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };
                var colTextoFecha = new PdfPCell(new Phrase("Fecha:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
                var colFecha = new PdfPCell(new Phrase("15/07/2020", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colTextoNoReporte);
                table.AddCell(colNoReporte);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoFecha);
                table.AddCell(colFecha);
                table.AddCell(celdaVacia);


                var colTextoPlaza = new PdfPCell(new Phrase("Plaza de Cobro", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };
                var colPlaza = new PdfPCell(new Phrase("001 TLALPAN", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };
                var colTextoHoraInicio = new PdfPCell(new Phrase("Hora INICIO:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
                var colHoraInicio = new PdfPCell(new Phrase("11:15", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colTextoPlaza);
                table.AddCell(colPlaza);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoHoraInicio);
                table.AddCell(colHoraInicio);
                table.AddCell(celdaVacia);

                var colTextoUbicacion = new PdfPCell(new Phrase("Ubicación", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };
                var colUbicacion = new PdfPCell(new Phrase("B01", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 2 };
                var colTextoHoraFin = new PdfPCell(new Phrase("Hora FIN:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };
                var colHoraFin = new PdfPCell(new Phrase("12:20", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2 };

                table.AddCell(colTextoUbicacion);
                table.AddCell(colUbicacion);
                table.AddCell(celdaVacia);
                table.AddCell(colTextoHoraFin);
                table.AddCell(colHoraFin);
                table.AddCell(celdaVacia);

                var colTextoPersonaProsis = new PdfPCell(new Phrase("Técnico Responsable PROSIS:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };
                var colPersonaProsis = new PdfPCell(new Phrase("PRUEBA", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                table.AddCell(celdaVacia);
                table.AddCell(colTextoPersonaProsis);
                table.AddCell(colPersonaProsis);
                table.AddCell(celdaVacia);

                var colTextoPersonaCapufe = new PdfPCell(new Phrase("Persona de CAPUFE que recibe: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };
                var colPersonaCapufe = new PdfPCell(new Phrase("LIC RAFAEL CASTREJON SALAZAR", letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 2, Colspan = 3 };

                table.AddCell(celdaVacia);
                table.AddCell(colTextoPersonaCapufe);
                table.AddCell(colPersonaCapufe);
                table.AddCell(celdaVacia);

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

        private IElement TablaDescripcion()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 20.67f, 20.67f, 24.67f, 8.67f, 8.67f, 8.67f }) { WidthPercentage = 100f };
                var celdaVacia = new PdfPCell() { Border = 0 };
                //ESPACIO
                for (int i = 0; i < 12; i++)
                    table.AddCell(celdaVacia);

                var colEquipoCarril = new PdfPCell(new Phrase("EQUIPO", letraoNegritaChica))
                {
                    BackgroundColor = BaseColor.LightGray,
                    Border = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 2
                };

                var colComponente = new PdfPCell(new Phrase("COMPONENTE", letraoNegritaChica))
                {
                    BackgroundColor = BaseColor.LightGray,
                    Border = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 2
                };
                var colActividades = new PdfPCell(new Phrase("ACTIVIDADES", letraoNegritaChica))
                {
                    BackgroundColor = BaseColor.LightGray,
                    Border = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 2
                };
                var colFrecuencia = new PdfPCell(new Phrase("FRECUENCIA", letraoNegritaChica))
                {
                    BackgroundColor = BaseColor.LightGray,
                    Border = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 2
                };
                var colUbicacion = new PdfPCell(new Phrase("UBICACIÓN", letraoNegritaChica))
                {
                    BackgroundColor = BaseColor.LightGray,
                    Border = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 2
                };
                var colEstatus = new PdfPCell(new Phrase("ESTATUS", letraoNegritaChica))
                {
                    BackgroundColor = BaseColor.LightGray,
                    Border = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 2
                };

                table.AddCell(colEquipoCarril);
                table.AddCell(colComponente);
                table.AddCell(colActividades);
                table.AddCell(colFrecuencia);
                table.AddCell(colUbicacion);
                table.AddCell(colEstatus);

                foreach (var equipo in equipos)
                {
                    var colEquipo = new PdfPCell(new Phrase(equipo.Nombre, letraNormalChica))
                    {
                        BorderWidth = 1,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 3,
                        Rowspan = equipo.Componentes.Count
                    };
                    table.AddCell(colEquipo);
                    foreach (var componente in equipo.Componentes)
                    {
                        table.AddCell(new PdfPCell
                        {
                            Phrase = new Phrase(componente.Nombre, letraNormalChica),
                            BorderWidth = 1,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 3
                        });
                        table.AddCell(new PdfPCell
                        {
                            Phrase = new Phrase(componente.Actividad, letraNormalChica),
                            BorderWidth = 1,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 3
                        });
                        table.AddCell(new PdfPCell
                        {
                            Phrase = new Phrase(Convert.ToString(componente.Frecuencia), letraNormalChica),
                            BorderWidth = 1,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 3
                        });
                        table.AddCell(new PdfPCell
                        {
                            Phrase = new Phrase(componente.Ubicacion, letraNormalChica),
                            BorderWidth = 1,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 3
                        });
                        table.AddCell(new PdfPCell
                        {
                            Phrase = new Phrase(componente.Estatus ? "OK" : "NO", letraNormalChica),
                            BorderWidth = 1,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 3
                        });
                    }
                }

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaDescripcion", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaDescripcion", 3);
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
                for (int i = 0; i < 16; i++)
                {
                    table.AddCell(celdaVacia);
                }

                var colTextoMotivo = new PdfPCell(new Phrase("Motivo:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };

                table.AddCell(colTextoMotivo);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                for (int i = 0; i < 8; i++)
                {
                    table.AddCell(celdaVacia);
                }


                var colTextoModulo = new PdfPCell(new Phrase("Sin acceso a Módulo de Fallas: ", letraNormalMediana)) { Border = 0, Colspan = 2, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 1 };
                var colBoolModulo = new PdfPCell(new Phrase("X", letraNormalMediana)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                var colTextoSinConexion = new PdfPCell(new Phrase("Sin conexión: ", letraNormalMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 1 };
                var colBoolSinConexion = new PdfPCell(new Phrase("X", letraNormalMediana)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
                var colTextoOtros = new PdfPCell(new Phrase("Otros: ", letraNormalMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 1 };
                var colBoolOtros = new PdfPCell(new Phrase("X", letraNormalMediana)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };

                table.AddCell(colTextoModulo);
                table.AddCell(colBoolModulo);
                table.AddCell(colTextoSinConexion);
                table.AddCell(colBoolSinConexion);
                table.AddCell(colTextoOtros);
                table.AddCell(colBoolOtros);
                table.AddCell(celdaVacia);

                for (int i = 0; i < 8; i++)
                {
                    table.AddCell(celdaVacia);
                }

                var colTextoObservaciones = new PdfPCell(new Phrase("Observaciones: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1 };

                table.AddCell(colTextoObservaciones);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);
                table.AddCell(celdaVacia);

                var celdaObservaciones = new PdfPCell(new Phrase("What is Lorem Ipsum ? Lorem Ipsum is simply dummy text of the printing and typesetting industry.Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", letraNormalMediana)) { BorderWidth = 0, VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_JUSTIFIED, Colspan = 8 };

                table.AddCell(celdaObservaciones);

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaObservaciones", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaObservaciones", 3);
                return null;
            }   
        }

        private IElement TablaFirmas()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 40f, 20f, 40f }) { WidthPercentage = 100f };

                var celdaVacia = new PdfPCell() { Border = 0, FixedHeight = 30f };
                for (int i = 0; i < 6; i++)
                    table.AddCell(celdaVacia);

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
                var celdaVaciaFirmas = new PdfPCell() { Border = 0 };
                table.AddCell(colNombreTecnico);
                table.AddCell(celdaVaciaFirmas);
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
                table.AddCell(celdaVaciaFirmas);
                table.AddCell(colEncargado);

                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaFirmas", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaFirmas", 3);
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

        private string MesActual() { return new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(DateTime.Now.Month); }

        private string[] TipoDeReporte(int tipo)
        {
            
            switch (tipo)
            {
                case 1:
                    return new string[] { "S", "semanal", "semanales", "plaza" };
                case 2:
                    return new string[] { "M", "mensual", "mensuales", "plaza" };
                case 3:
                    return new string[] { "T", "trimestral", "trimestrales", "plaza" };
                case 4:
                    return new string[] { "SM", "semestral", "semestrales", "plaza" };
                case 5:
                    return new string[] { "A", "anual", "anuales", "plaza" };
                case 6:
                    return new string[] { "S", "semanal", "semanales", "carril" };
                case 7:
                    return new string[] { "M", "mensual", "mensuales", "carril" };
                case 8:
                    return new string[] { "T", "trimestral", "trimestrales", "carril" };
                case 9:
                    return new string[] { "SM", "semestral", "semestrales", "carril" };
                case 10:
                    return new string[] { "A", "anual", "anuales", "carril" };
                default: 
                    return null;
            }
        }
        #endregion

        #region Clase de prueba
        public class Equipo
        {
            public string Nombre { get; set; }

            public List<Componente> Componentes { get; set; }
        }

        public class Componente
        {
            public string Nombre { get; set; }

            public string Actividad { get; set; }

            public int Frecuencia { get; set; }

            public string Ubicacion { get; set; }

            public bool Estatus { get; set; }
        }
        List<Equipo> equipos = new List<Equipo>
        {
            new Equipo
            {
                Nombre = "Semáforo de Marquesina",
                Componentes = new List<Componente>
                {
                    new Componente
                    {
                        Nombre = "Interruptor / Control",
                        Actividad = "Limpieza de tarjeta de control",
                        Frecuencia = 5,
                        Ubicacion = "1 CARRIL",
                        Estatus = true
                    },
                    new Componente
                    {
                        Nombre = "Interruptor / Control",
                        Actividad = "Verificación de conexiones en tarjeta de control",
                        Frecuencia = 5,
                        Ubicacion = "1 CARRIL",
                        Estatus = false
                    }
                }
            },
            new Equipo
            {
                Nombre = "Monitores (PC)",
                Componentes = new List<Componente>
                {
                    new Componente
                    {
                        Nombre = "Gabinete",
                        Actividad = "Limpieza exterior",
                        Frecuencia = 1,
                        Ubicacion = "5 ADMIN",
                        Estatus = true
                    }
                }
            },
            new Equipo
            {
                Nombre = "Estación central de interfon, al interfon de carril y Teléfono IP (CISCO / Alcatel)",
                Componentes = new List<Componente>
                {
                    new Componente
                    {
                        Nombre = "Estación Maestra",
                        Actividad = "Limpieza exterior",
                        Frecuencia = 1,
                        Ubicacion = "6 CSPT",
                        Estatus = true
                    },
                    new Componente
                    {
                        Nombre = "Fuente de alimentación",
                        Actividad = "Limpieza de gabinete de fuente de alimentación",
                        Frecuencia = 2,
                        Ubicacion = "6 CSPT",
                        Estatus = false
                    },
                    new Componente
                    {
                        Nombre = "Teléfono IP Carril",
                        Actividad = "Limpieza Revisión integral de bocina y protector cubre polvo",
                        Frecuencia = 3,
                        Ubicacion = "5 ADMIN",
                        Estatus = true
                    },
                    new Componente
                    {
                        Nombre = "Pruebas funcionales",
                        Actividad = "Pruebas de intercomunicación",
                        Frecuencia = 4,
                        Ubicacion = "5 ADMIN",
                        Estatus = true
                    },
                    new Componente
                    {
                        Nombre = "Pruebas funcionales",
                        Actividad = "Limpieza Diagnóstico de estado físico y operativo",
                        Frecuencia = 5,
                        Ubicacion = "5 ADMIN",
                        Estatus = false
                    }
                }
            },
        };
        
        #endregion
    }
}
