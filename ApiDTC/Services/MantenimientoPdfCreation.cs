﻿namespace ApiDTC.Services
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
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion
        #region Logo
        #endregion
        #endregion

        #region Constructors

        public MantenimientoPdfCreation(string clavePlaza, DataTable tableHeader, DataTable tableActivities, ApiLogger apiLogger, int tipo, string noReporte)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _tableActivities = tableActivities;
            _tipo = tipo;
            _temporal = TipoDeReporte(_tipo);
            _noReporte = noReporte;
        }

        #endregion
        #region Methods
        public Response NewPdf()
        {
            DateTime now = DateTime.Now;
            string directory = $@"C:\Bitacora\{_clavePlaza}\Reportes\{_noReporte}";
            string filename = $"{_noReporte}.pdf";
            
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

                    List<Equipo> equipos = CreacionListaActividades();
                    TablaDescripcion(doc, equipos, EquiposPivote(equipos));
                    doc.Add(TablaObservaciones());
                    doc.Add(new Phrase(" "));
                    doc.Add(new Phrase(" "));
                    doc.Add(new Phrase(" "));

                    //Pdf fotografías evidencia
                    /*string directoryImgs = Path.Combine(directory, "Imgs");
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
                    }*/
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
                
                PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                table.AddCell(colLogo);
                CeldasVacias(1, table);

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
                var colTitulo = new PdfPCell(new Phrase(_textoTitulo, letraoNegritaGrande)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 4, Border = 0 };
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

                PdfPCell colLogo = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                table.AddCell(colLogo);
                CeldasVacias(3, table);


                var celdaSalto = new PdfPCell() { Colspan = 4, Border = 0 };

                var colTitulo = new PdfPCell(new Phrase(_textoTitulo, letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 2 };
                CeldasVacias(1, table);
                table.AddCell(colTitulo);
                CeldasVacias(1, table);


                var colPlaza = new PdfPCell(new Phrase("PLAZA (PONER PALMILLAS)", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 2 };
                CeldasVacias(1, table);
                table.AddCell(colPlaza); ;
                CeldasVacias(1, table);


                var colReporte = new PdfPCell(new Phrase($"REPORTE: (pág. {pagina})", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 5, PaddingRight = 20, PaddingLeft = 20, Colspan = 2 };
                CeldasVacias(1, table);
                table.AddCell(colReporte);
                CeldasVacias(1, table);
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

                string valorReporte = Convert.ToString(_tableHeader.Rows[0]["NumeroReporte"]);
                var colTextoNoReporte = new PdfPCell(new Phrase("No. de Reporte:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 2 };
                var colNoReporte = new PdfPCell(new Phrase(valorReporte, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };
                
                string valorFecha  = Convert.ToString(_tableHeader.Rows[0]["Fecha"]).Substring(0, 8);
                var colTextoFecha = new PdfPCell(new Phrase("Fecha:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };
                var colFecha = new PdfPCell(new Phrase(valorFecha, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colTextoNoReporte);
                table.AddCell(colNoReporte);
                CeldasVacias(1, table);
                table.AddCell(colTextoFecha);
                table.AddCell(colFecha);
                CeldasVacias(1, table);

                string valorPlaza = Convert.ToString(_tableHeader.Rows[0]["Plaza"]);
                var colTextoPlaza = new PdfPCell(new Phrase("Plaza de Cobro:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 2 };
                var colPlaza = new PdfPCell(new Phrase(valorPlaza, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };
                
                var colTextoHoraInicio = new PdfPCell(new Phrase("Hora INICIO:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };
                string valorHoraInicio = Convert.ToString(_tableHeader.Rows[0]["Inicio"]);
                var colHoraInicio = new PdfPCell(new Phrase(valorHoraInicio, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colTextoPlaza);
                table.AddCell(colPlaza);
                CeldasVacias(1, table);
                table.AddCell(colTextoHoraInicio);
                table.AddCell(colHoraInicio);
                CeldasVacias(1, table);

                var colTextoUbicacion = new PdfPCell(new Phrase("Ubicación:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 2 };
                string valorUbicacion = Convert.ToString(_tableHeader.Rows[0]["Ubicacion"]);
                var colUbicacion = new PdfPCell(new Phrase(valorUbicacion, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 2 };

                var colTextoHoraFin = new PdfPCell(new Phrase("Hora FIN:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4 };
                string valorHoraFin = Convert.ToString(_tableHeader.Rows[0]["Fin"]);
                var colHoraFin = new PdfPCell(new Phrase(valorHoraFin, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2 };

                table.AddCell(colTextoUbicacion);
                table.AddCell(colUbicacion);
                CeldasVacias(1, table);
                table.AddCell(colTextoHoraFin);
                table.AddCell(colHoraFin);
                CeldasVacias(1, table);

                string valorProsis = Convert.ToString(_tableHeader.Rows[0]["TecnicoProsis"]);
                var colTextoPersonaProsis = new PdfPCell(new Phrase("Técnico Responsable PROSIS:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 3 };
                var colPersonaProsis = new PdfPCell(new Phrase(valorProsis, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                CeldasVacias(1, table);
                table.AddCell(colTextoPersonaProsis);
                table.AddCell(colPersonaProsis);
                CeldasVacias(1, table);

                string valorCapufe = Convert.ToString(_tableHeader.Rows[0]["PersonalCapufe"]);
                var colTextoPersonaCapufe = new PdfPCell(new Phrase("Persona de CAPUFE que recibe:", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 4, Colspan = 3 };
                var colPersonaCapufe = new PdfPCell(new Phrase(valorCapufe, letraNormalMediana)) { BorderWidthBottom = 1, BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Padding = 2, Colspan = 3 };

                CeldasVacias(1, table);
                table.AddCell(colTextoPersonaCapufe);
                table.AddCell(colPersonaCapufe);
                CeldasVacias(1, table);

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

        private void TablaDescripcion(Document doc, List<Equipo> equipos, List<Equipo> pivotes)
        {
            try
            {
                while(pivotes.Count != 0)
                {
                    PdfPTable table = new PdfPTable(new float[] { 19.17f, 19.17f, 26.67f, 8.67f, 9.67f, 8.67f }) { WidthPercentage = 100f };
                    CeldasVacias(12, table);

                    table.AddCell(new PdfPCell(new Phrase("EQUIPO", letraoNegritaChica))
                    {
                        BackgroundColor = BaseColor.LightGray,
                        Border = 1,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_CENTER,
                        Padding = 2
                    });
                    table.AddCell(new PdfPCell(new Phrase("COMPONENTE", letraoNegritaChica))
                    {
                        BackgroundColor = BaseColor.LightGray,
                        Border = 1,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_CENTER,
                        Padding = 2
                    });
                    table.AddCell(new PdfPCell(new Phrase("ACTIVIDADES", letraoNegritaChica))
                    {
                        BackgroundColor = BaseColor.LightGray,
                        Border = 1,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_CENTER,
                        Padding = 2
                    });
                    table.AddCell(new PdfPCell(new Phrase("FRECUENCIA", letraoNegritaChica))
                    {
                        BackgroundColor = BaseColor.LightGray,
                        Border = 1,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_CENTER,
                        Padding = 2
                    });
                    table.AddCell(new PdfPCell(new Phrase("UBICACIÓN", letraoNegritaChica))
                    {
                        BackgroundColor = BaseColor.LightGray,
                        Border = 1,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_CENTER,
                        Padding = 2
                    });
                    table.AddCell(new PdfPCell(new Phrase("ESTATUS", letraoNegritaChica))
                    {
                        BackgroundColor = BaseColor.LightGray,
                        Border = 1,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_CENTER,
                        Padding = 2
                    });

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
                                Phrase = new Phrase(componente.Estatus, letraNormalChica),
                                BorderWidth = 1,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                Padding = 3
                            });
                        }
                        if (pivotes.Contains(equipo))
                        {
                            pivotes.Remove(equipo);
                            doc.Add(table);
                            //doc.NewPage();
                        }
                    }
                }
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaDescripcion", 5);
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MatenimientoPdfCreation: TablaDescripcion", 3);
            }
        }

        private IElement TablaObservaciones()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                CeldasVacias(8, table);


                var colTextoObservaciones = new PdfPCell(new Phrase("Observaciones: ", letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 2 };

                table.AddCell(colTextoObservaciones);
                CeldasVacias(6, table);

                var celdaObservaciones = SeparacionObservaciones(Convert.ToString(_tableHeader.Rows[0]["Observaciones"]));
                int celdasTotalesObservaciones = 0;
                foreach (var linea in celdaObservaciones)
                {
                    celdasTotalesObservaciones +=1;
                    var celdaLinea = new PdfPCell(new Phrase(Convert.ToString(linea), letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                
                for (int i = 0; i < 3 - celdasTotalesObservaciones; i++)
                {
                    var celdaLinea = new PdfPCell(new Phrase("", letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
                CeldasVacias(16, table);
                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MantenimientoPdfCreation: TablaObservaciones", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MantenimientoPdfCreation: TablaObservaciones", 3);
                return null;
            }      
        }

        private IElement TablaFirmas()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 30f, 10f, 30f, 10f, 30f }) { WidthPercentage = 100f };

                CeldasVacias(15, table);
                var celdaVaciaFirmas = new PdfPCell() { Border = 0, FixedHeight = 30 };
                for (int i = 0; i < 4; i++)
                    table.AddCell(celdaVaciaFirmas);
                table.AddCell(new PdfPCell() {BorderWidthTop = 1, BorderWidthBottom = 0, BorderWidthLeft = 1, BorderWidthRight = 1, FixedHeight = 30, Rowspan = 3 });
                for (int i = 0; i < 8; i++)
                    table.AddCell(celdaVaciaFirmas);
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
                string valorTecnicoProsis = _tipo == 1 ? Convert.ToString(_tableHeader.Rows[0]["TecnicoProsis"]) : Convert.ToString(_tableHeader.Rows[0]["TecnicoProsis"]);
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
                _apiLogger.WriteLog(_clavePlaza, ex, "MantenimientoPdfCreation: TablaFirmas", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "MantenimientoPdfCreation: TablaFirmas", 3);
                return null;
            }
        }

        public void CeldasVacias(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell() { Border = 0 });
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

        private List<Equipo> EquiposPivote(List<Equipo> equipos)
        {
            List<Equipo> equiposFinales = new List<Equipo>();
            int conteo = 0;
            foreach (Equipo item in equipos)
            {
                foreach (Componente componente in item.Componentes)
                {
                    conteo = conteo + 1;
                    //Si hay menos de 38 componentes, y la lista no ha registrado el item del conteo idéntico al número de componentes
                    if (_tableActivities.Rows.Count < 38 && !equiposFinales.Contains(item) && conteo == _tableActivities.Rows.Count)
                    {
                        equiposFinales.Add(item);
                        break;
                    }

                    else if (_tableActivities.Rows.Count == 38 && !equiposFinales.Contains(item) && conteo == 38)
                    {
                        equiposFinales.Add(item);
                        break;
                    }
                    else if (_tableActivities.Rows.Count > 38)
                    {
                        if (conteo == 38 && !equiposFinales.Contains(item))
                            equiposFinales.Add(item);
                        if (_tableActivities.Rows.Count < 81)
                        {
                            if (conteo == _tableActivities.Rows.Count && !equiposFinales.Contains(item))
                            {
                                equiposFinales.Add(item);
                                break;
                            }
                        }
                        else if (_tableActivities.Rows.Count == 81)
                        {
                            if (conteo == 81 && !equiposFinales.Contains(item))
                            {
                                equiposFinales.Add(item);
                                break;
                            }
                        }
                        else
                        {
                            if (_tableActivities.Rows.Count < 91)
                            {
                                if (conteo == _tableActivities.Rows.Count && !equiposFinales.Contains(item))
                                {
                                    equiposFinales.Add(item);
                                    break;
                                }
                            }
                            else if (_tableActivities.Rows.Count == 91 && !equiposFinales.Contains(item) && conteo == 91)
                            {
                                equiposFinales.Add(item);
                                break;
                            }
                            else
                            {
                                if (53 % (conteo - 38) == 0 && !equiposFinales.Contains(item))
                                    equiposFinales.Add(item);
                                else if (conteo == _tableActivities.Rows.Count && !equiposFinales.Contains(item))
                                    equiposFinales.Add(item);
                            }
                        }
                    }
                }
            }
            return equiposFinales;
        }
        
        private List<string> SeparacionObservaciones(string observaciones)
        {
            List<string> lineaObservaciones = new List<string>();
            if(observaciones.Length < 125)
            {
                lineaObservaciones.Add(observaciones);
                return lineaObservaciones;
            }

            var palabras = observaciones.Split(' ');
            string linea = string.Empty;
            foreach (var palabra in palabras)
            {
                linea += $" {palabra}";
                if(linea.Length > 115)
                {
                    lineaObservaciones.Add(linea);
                    linea = string.Empty;
                }
                if(palabra == palabras[palabras.Length - 1] && linea.Length < 115)
                {
                    lineaObservaciones.Add(linea);
                    linea = string.Empty;
                }
            }
            return lineaObservaciones;
        }
        
        private List<Equipo> CreacionListaActividades()
        {
            List<Equipo> equipos = new List<Equipo>();
            int conteo = 0;

            foreach (DataRow row in _tableActivities.Rows)
            {
                conteo += 1;
                if (!equipos.Exists(x => x.Nombre.Equals(Convert.ToString(row["Equipo"]))))
                {
                    Equipo equipo = new Equipo();
                    equipo.Nombre = Convert.ToString(row["Equipo"]);
                    equipo.Componentes = new List<Componente>
                    {
                        new Componente
                        {
                            Nombre = Convert.ToString(row["Componente"]),
                            Actividad = Convert.ToString(row["Actividad"]),
                            Frecuencia = Convert.ToString(row["Frecuencia"]),
                            Ubicacion = Convert.ToString(row["Ubicacion"]),
                            Estatus = Convert.ToString(row["Realizada"])
                        }
                    };
                    equipos.Add(equipo);
                }
                else
                {
                    Equipo equipo = equipos.Find(x => x.Nombre.Equals(Convert.ToString(row["Equipo"])));

                    equipo.Componentes.Add(new Componente
                    {
                        Nombre = Convert.ToString(row["Componente"]),
                        Actividad = Convert.ToString(row["Actividad"]),
                        Frecuencia = Convert.ToString(row["Frecuencia"]),
                        Ubicacion = Convert.ToString(row["Ubicacion"]),
                        Estatus = Convert.ToString(row["Realizada"])
                    });
                }
            }
            return equipos;
        }
        #endregion
    }
}
