using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using ApiDTC.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ApiDTC.Services
{
    public class AnexosPdfCreation
    {

        #region Attributes

        private readonly DataTable _tableAnexo;

        private readonly DataTable _tableComponentesNuevos;

        private readonly DataTable _tableComponentesDañados;

        private readonly string _clavePlaza;

        private readonly string _referenciaAnexo;

        private readonly string _referenceNumber;

        private readonly ApiLogger _apiLogger;

        private readonly DateTime fechaSiniestro;

        private readonly DateTime fechaApertura;

        private readonly DateTime fechaCierre;

        private readonly DateTime fechaInicio;

        private readonly DateTime fechaSolicitud;

        private readonly bool _mostrarMarcaDeAgua;
        #endregion

        #region Constructors

        public AnexosPdfCreation(string clavePlaza, string referenciaAnexo, string referenceNumber, DataTable tableAnexo, DataTable componentesDañados, DataTable componentesNuevos, ApiLogger apiLogger, bool mostrarMarcaDeAgua = false)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _tableAnexo = tableAnexo;
            _tableComponentesNuevos = componentesNuevos;
            _tableComponentesDañados = componentesDañados;
            _referenciaAnexo = referenciaAnexo;
            _referenceNumber = referenceNumber;
            _mostrarMarcaDeAgua = mostrarMarcaDeAgua;

            if (!DBNull.Value.Equals(_tableAnexo.Rows[0]["SinisterDate"]))
                fechaSiniestro = Convert.ToDateTime(_tableAnexo.Rows[0]["SinisterDate"]);

            if (!DBNull.Value.Equals(_tableAnexo.Rows[0]["FechaApertura"]))
                fechaApertura = Convert.ToDateTime(_tableAnexo.Rows[0]["FechaApertura"]);

            if (!DBNull.Value.Equals(_tableAnexo.Rows[0]["FechaCierre"]))
                fechaCierre = Convert.ToDateTime(_tableAnexo.Rows[0]["FechaCierre"]);

            if(!DBNull.Value.Equals(_tableAnexo.Rows[0]["FechaOficioInicio"]))
                fechaInicio = Convert.ToDateTime(_tableAnexo.Rows[0]["FechaOficioInicio"]);

            if (!DBNull.Value.Equals(_tableAnexo.Rows[0]["FechaSolicitudInicio"]))
                fechaSolicitud = Convert.ToDateTime(_tableAnexo.Rows[0]["FechaSolicitudInicio"]);
        }

        #endregion

        #region BaseFont

        public static string path = $@"{System.Environment.CurrentDirectory}\Media\Montserrat\";

        public static BaseFont BoldGrande = BaseFont.CreateFont(path + "Montserrat Black 900.ttf", BaseFont.WINANSI, true);
        public static BaseFont NormalMediana = BaseFont.CreateFont(path + "Montserrat Medium 500.ttf", BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(path + "Montserrat Bold 700.ttf", BaseFont.WINANSI, true);
        public static BaseFont NegritaMedianaIta = BaseFont.CreateFont(path + "Montserrat SemiBold Italic 600.ttf", BaseFont.WINANSI, true);

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

        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);

        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

        public static iTextSharp.text.Font letraNegritaItalicaMedianaRoja = new iTextSharp.text.Font(NegritaMedianaIta, 8f, iTextSharp.text.Font.BOLD, BaseColor.Red);
        //public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNegritaItalicaMediana = new iTextSharp.text.Font(NegritaMedianaIta, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);

        public static iTextSharp.text.Font letraNormalMedianaRoja = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Red);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion

        #region Methods
        public Response NewPdfA(string folder)
        {
            string directory, filename, path;
            directory = $@"{folder}\{_clavePlaza.ToUpper()}\DTC\{_referenceNumber}\Anexos";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            filename = $"AnexoA_{_referenciaAnexo}.pdf";

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
                    string Tipo = "A";
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
                    writer.PageEvent = new PageEventHelperVerticalAnexo(Tipo, _tableAnexo, _mostrarMarcaDeAgua);
                    writer.Open();
                    doc.Open();

                    doc.Add(TablaLegalA());
                    doc.Add(TablaComponentesDañados(writer));

                    var posicionEnY = writer.GetVerticalPosition(true);
                    doc.Add(TablaComponentesNuevos(writer, doc, posicionEnY));

                    posicionEnY = writer.GetVerticalPosition(true);
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
                _apiLogger.WriteLog(_clavePlaza, ex, "AnexoAPdfCreation: NewPdf", 2);
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

        public Response NewPdfB(string folder)
        {
            string directory, filename, path;
            directory = $@"{folder}\{_clavePlaza.ToUpper()}\DTC\{_referenceNumber}\Anexos";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            filename = $"AnexoB_{_referenciaAnexo}.pdf";

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
                _apiLogger.WriteLog(_clavePlaza, ex, "AnexoBPdfCreation: NewPdf", 2);
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
                    string Tipo = "B";
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
                    writer.PageEvent = new PageEventHelperVerticalAnexo(Tipo, _tableAnexo, _mostrarMarcaDeAgua);
                    writer.Open();
                    doc.Open();

                    doc.Add(TablaLegalB());
                    doc.Add(TablaComponentesDañados(writer));

                    var posicionEnY = writer.GetVerticalPosition(true);
                    doc.Add(TablaComponentesNuevos(writer, doc, posicionEnY));

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
                _apiLogger.WriteLog(_clavePlaza, ex, "AnexoBPdfCreation: NewPdf", 2);
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

        private IElement TablaLegalA()
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };


                Chunk Uno = new Chunk("SE LEVANTA LA PRESENTE ACTA, PARA HACER CONSTAR EL SERVICIO DE MANTENIMIENTO ", letraNormalMediana);
                Chunk Uno2 = new Chunk("CORRECTIVO (SINIESTRO, ACCIDENTE VEHICULAR, DESCARGA ELÉCTRICA, ETC.) ", letraoNegritaMediana);
                Chunk Uno3 = new Chunk("REALIZADO AL EQUIPO DE CONTROL DE TRANSITO DE ", letraNormalMediana);

                string carril_es = appendRows(_tableAnexo, "Carril");
                Chunk Uno4 = carril_es.Length > 3 ? new Chunk("CARRILES " + carril_es.ToUpper() + ", ", letraoNegritaMediana) : new Chunk("CARRIL " + carril_es.ToUpper() + ", ", letraoNegritaMediana);

                Chunk Uno5 = new Chunk("EN LA PLAZA DE COBRO ", letraNormalMediana);
                Chunk Uno6 = new Chunk(_tableAnexo.Rows[0]["Plaza"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Uno7 = new Chunk("PERTENECIENTE A LA ", letraNormalMediana);

                Chunk Uno8 = _tableAnexo.Rows[0]["Region"].ToString() == "Estado de México" ? new Chunk("UNIDAD REGIONAL ESTADO DE MÉXICO.", letraoNegritaMediana) : new Chunk(_tableAnexo.Rows[0]["Region"].ToString().ToUpper() + ".", letraoNegritaMediana);

                var parrafoUno = new Paragraph();
                parrafoUno.SetLeading(0, 1.4f);
                parrafoUno.Add(Uno);
                parrafoUno.Add(Uno2);
                parrafoUno.Add(Uno3);
                parrafoUno.Add(Uno4);
                parrafoUno.Add(Uno5);
                parrafoUno.Add(Uno6);
                parrafoUno.Add(Uno7);
                parrafoUno.Add(Uno8);
                parrafoUno.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Dos1 = new Chunk("EN LA CIUDAD DE ", letraNormalMediana);
                Chunk Dos2 = new Chunk(_tableAnexo.Rows[0]["Ciudad"].ToString().ToUpper() + ", " + _tableAnexo.Rows[0]["Estado"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos3 = new Chunk("SIENDO LAS ", letraNormalMediana);
                Chunk Dos4 = new Chunk(fechaApertura.ToString("hh:mm", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + " HRS. ", letraoNegritaMediana);
                //Chunk Dos4 = new Chunk("16:30 HRS. ", letraoNegritaMediana);
                Chunk Dos5 = new Chunk("DEL DÍA ", letraNormalMediana);
                Chunk Dos6 = new Chunk(fechaApertura.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + ", ", letraoNegritaMediana);
                //Chunk Dos6 = new Chunk("10 DE DICIEMBRE DE 2021, ", letraoNegritaMediana);
                Chunk Dos7 = new Chunk("EL ", letraNormalMediana);
                Chunk Dos8 = new Chunk(_tableAnexo.Rows[0]["Admin"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos9 = new Chunk("ADMINISTRADOR DE LA PLAZA DE COBRO, EL ", letraNormalMediana);
                Chunk Dos10 = new Chunk("C. " + _tableAnexo.Rows[0]["Supervisor"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos11 = new Chunk("TÉCNICO REPRESENTANTE DE LA EMPRESA PRESTADORA DE SERVICIOS PROYECTOS Y SISTEMAS INFORMATICOS S.A. DE C.V. Y LOS ", letraNormalMediana);
                Chunk Dos12 = new Chunk(_tableAnexo.Rows[0]["Testigo Uno"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos13 = new Chunk("Y ", letraNormalMediana);
                Chunk Dos14 = new Chunk(_tableAnexo.Rows[0]["Testigo Dos"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos15 = new Chunk("TESTIGOS DE ASISTENCIA, PARA HACER CONSTAR QUE LA FALLA DEL EQUIPO DE ", letraNormalMediana);

                Chunk Dos16 = carril_es.Length > 3 ? new Chunk("LOS CARRILES " + carril_es.ToUpper() + ", ", letraoNegritaMediana) : new Chunk("CARRIL " + carril_es.ToUpper() + ", ", letraoNegritaMediana);

                Chunk Dos17 = new Chunk("REPORTADA CON No. DE ACUSE/FOLIO ", letraNormalMediana);

                string fallo_s = appendRows(_tableAnexo, "No. de Fallo");
                Chunk Dos18 = new Chunk(fallo_s.ToUpper() + ", ", letraoNegritaMediana);

                Chunk Dos19 = new Chunk("DE FECHA ", letraNormalMediana);
                Chunk Dos20 = new Chunk(fechaSiniestro.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + "; ", letraoNegritaMediana);
                //Chunk Dos20 = new Chunk("18 DE JUNIO DE 2021; ", letraoNegritaMediana);
                Chunk Dos21 = new Chunk("FUE REPARADA EL DÍA ", letraNormalMediana);
                Chunk Dos22 = new Chunk(fechaCierre.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + ", ", letraoNegritaMediana);
                //Chunk Dos22 = new Chunk("10 DE DICIEMBRE DE 2021, ", letraoNegritaMediana);
                Chunk Dos23 = new Chunk("DICHA FALLA CONSISTIÓ EN EL DAÑO A ", letraNormalMediana);

                string componente_es = appendRows(_tableComponentesDañados, "Componente");
                Chunk Dos24 = new Chunk(componente_es.ToUpper() + ", ", letraoNegritaMediana);

                Chunk Dos25 = new Chunk("Y FUE PROVOCADA POR ", letraNormalMediana);

                string descripcion_es = appendRows(_tableAnexo, "Descripcion");
                Chunk Dos26 = new Chunk(descripcion_es.ToUpper() + ", ", letraoNegritaMediana);

                Chunk Dos27 = new Chunk("OCURRIDO EL ", letraNormalMediana);
                Chunk Dos28 = new Chunk(fechaSiniestro.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + "; ", letraoNegritaMediana);
                //Chunk Dos28 = new Chunk("18 DE JUNIO DE 2021; ", letraoNegritaMediana);
                Chunk Dos29 = new Chunk("PARA CUYO EFECTO FUE NECESARIO REPONER LAS PARTES QUE A CONTINUACIÓN SE DETALLAN.", letraNormalMediana);
                var parrafoDos = new Paragraph();
                parrafoDos.SetLeading(0, 1.4f);

                parrafoDos.Add(Dos1);
                parrafoDos.Add(Dos2);
                parrafoDos.Add(Dos3);
                parrafoDos.Add(Dos4);
                parrafoDos.Add(Dos5);
                parrafoDos.Add(Dos6);
                parrafoDos.Add(Dos7);
                parrafoDos.Add(Dos8);
                parrafoDos.Add(Dos9);
                parrafoDos.Add(Dos10);
                parrafoDos.Add(Dos11);
                parrafoDos.Add(Dos12);
                parrafoDos.Add(Dos13);
                parrafoDos.Add(Dos14);
                parrafoDos.Add(Dos15);
                parrafoDos.Add(Dos16);
                parrafoDos.Add(Dos17);
                parrafoDos.Add(Dos18);
                parrafoDos.Add(Dos19);
                parrafoDos.Add(Dos20);
                parrafoDos.Add(Dos21);
                parrafoDos.Add(Dos22);
                parrafoDos.Add(Dos23);
                parrafoDos.Add(Dos24);
                parrafoDos.Add(Dos25);
                parrafoDos.Add(Dos26);
                parrafoDos.Add(Dos27);
                parrafoDos.Add(Dos28);
                parrafoDos.Add(Dos29);
                parrafoDos.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Tres = new Chunk("LOS EQUIPOS/COMPONENTES DAÑADOS EL ADMINISTRADOR DEBERÁ IDENTIFICAR Y EMBALAR, ENVIANDOLOS EN UN PERÍODO DE 5 DÍAS MÁXIMO AL ÁLMACÉN DE LA COORDINACION REGIONAL POR OFICIO, PARA SU DESTINO FINAL CONFORME LA NORMA CAPUFE.", letraNormalMediana);
                var parrafoTres = new Paragraph();
                parrafoTres.SetLeading(0, 1.4f);
                parrafoTres.Add(Tres);
                parrafoTres.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Cuatro = new Chunk("LOS TRABAJOS, INSTALACIÓN Y OPERACIÓN DE LA PLAZA DE COBRO QUEDAN A ", letraNormalMediana);
                Chunk Cuatro2 = new Chunk("ENTERA SATISFACCIÓN ", letraoNegritaMediana);
                Chunk Cuatro3 = new Chunk("DEL ADMINISTRADOR DE LA PLAZA DE COBRO Y/O ENCARGADO DE TURNO.", letraNormalMediana);
                var parrafoCuatro = new Paragraph();
                parrafoCuatro.SetLeading(0, 1.4f);
                parrafoCuatro.Add(Cuatro);
                parrafoCuatro.Add(Cuatro2);
                parrafoCuatro.Add(Cuatro3);
                parrafoCuatro.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk CiincoUno = new Chunk("NO. SINIESTRO Y/O NO. DE REPORTE. ", letraoNegritaMediana);
                string siniestro;
                if (!DBNull.Value.Equals(_tableAnexo.Rows[0]["No. de Siniestro"]) && _tableAnexo.Rows[0]["No. de Siniestro"].ToString() != "" && _tableAnexo.Rows[0]["No. de Siniestro"].ToString() != "S/N")
                    siniestro = _tableAnexo.Rows[0]["No. de Siniestro"].ToString().ToUpper();
                else
                    siniestro = _tableAnexo.Rows[0]["No. de Reporte"].ToString().ToUpper();
                Chunk CiincoDos = new Chunk(siniestro + " DE FECHA ", letraoNegritaMediana);

                Chunk CiincoTres = new Chunk(fechaSiniestro.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper(), letraoNegritaMediana);
                var parrafoCinco = new Paragraph();
                parrafoCinco.SetLeading(0, 1.4f);
                parrafoCinco.Add(CiincoUno);
                parrafoCinco.Add(CiincoDos);
                parrafoCinco.Add(CiincoTres);
                parrafoCinco.Alignment = Element.ALIGN_JUSTIFIED;

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

                var colParrafoCinco = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10
                };
                colParrafoCinco.AddElement(parrafoCinco);

                table.AddCell(colParrafoUno);
                table.AddCell(colParrafoDos);
                table.AddCell(colParrafoTres);
                table.AddCell(colParrafoCuatro);
                table.AddCell(colParrafoCinco);

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

        private IElement TablaLegalB()
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };

                Chunk Uno = new Chunk("SE LEVANTA LA PRESENTE ACTA, PARA HACER CONSTAR LA SUSTITUCIÓN DE COMPONENTES ", letraNormalMediana);
                Chunk Uno2 = new Chunk("POR FÍN DE VIDA ÚTIL/REUBICACIÓN/ACONDICIONAMIENTO", letraoNegritaMediana);
                Chunk Uno3 = new Chunk(", REALIZADO AL EQUIPO DE CONTROL DE TRANSITO ", letraNormalMediana);

                string carril_es = appendRows(_tableAnexo, "Carril");
                Chunk Uno4 = carril_es.Length > 3 ? new Chunk("DE CARRILES " + carril_es.ToUpper() + " ", letraoNegritaMediana) : new Chunk("DE CARRIL " + carril_es.ToUpper() + " ", letraoNegritaMediana);

                Chunk Uno5 = new Chunk("EN LA PLAZA DE COBRO ", letraNormalMediana);
                Chunk Uno6 = new Chunk(_tableAnexo.Rows[0]["Plaza"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Uno7 = new Chunk("PERTENECIENTE A LA ", letraNormalMediana);
                Chunk Uno8 = _tableAnexo.Rows[0]["Region"].ToString() == "Estado de México" ? new Chunk("UNIDAD REGIONAL ESTADO DE MÉXICO.", letraoNegritaMediana) : new Chunk(_tableAnexo.Rows[0]["Region"].ToString().ToUpper() + ".", letraoNegritaMediana);
                var parrafoUno = new Paragraph();
                parrafoUno.SetLeading(0, 1.4f);
                parrafoUno.Add(Uno);
                parrafoUno.Add(Uno2);
                parrafoUno.Add(Uno3);
                parrafoUno.Add(Uno4);
                parrafoUno.Add(Uno5);
                parrafoUno.Add(Uno6);
                parrafoUno.Add(Uno7);
                parrafoUno.Add(Uno8);
                parrafoUno.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Dos1 = new Chunk("EN LA CIUDAD ", letraNormalMediana);
                Chunk Dos2 = new Chunk(_tableAnexo.Rows[0]["Ciudad"].ToString().ToUpper() + ", " + _tableAnexo.Rows[0]["Estado"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos3 = new Chunk("SIENDO LAS ", letraNormalMediana);
                Chunk Dos4 = new Chunk(fechaApertura.ToString("hh:mm", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + " HRS. ", letraoNegritaMediana);
                Chunk Dos5 = new Chunk(" DEL DÍA ", letraNormalMediana);
                Chunk Dos6 = new Chunk(fechaApertura.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper(), letraoNegritaMediana);
                Chunk Dos7 = new Chunk(" EL ", letraNormalMediana);
                Chunk Dos8 = new Chunk(_tableAnexo.Rows[0]["Admin"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos9 = new Chunk("ADMINISTRADOR DE LA PLAZA DE COBRO, EL ", letraNormalMediana);
                Chunk Dos10 = new Chunk("C. " + _tableAnexo.Rows[0]["Supervisor"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos11 = new Chunk("TÉCNICO REPRESENTANTE DE LA EMPRESA PRESTADORA DE SERVICIOS PROYECTOS Y SISTEMAS INFORMATICOS S.A. DE C.V. Y LOS ", letraNormalMediana);
                Chunk Dos12 = new Chunk(_tableAnexo.Rows[0]["Testigo Uno"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos13 = new Chunk("Y EL ", letraNormalMediana);
                Chunk Dos14 = new Chunk(_tableAnexo.Rows[0]["Testigo Dos"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos15 = new Chunk("TESTIGOS DE ASISTENCIA, PARA HACER CONSTAR LA SUSTITUCIÓN DE COMPONENTES DEL EQUIPO DE CONTROL DE TRANSITO DE ", letraNormalMediana);

                Chunk Dos16 = carril_es.Length > 3 ? new Chunk("LOS CARRILES " + carril_es.ToUpper() + ", ", letraoNegritaMediana) : new Chunk("CARRIL " + carril_es.ToUpper() + ", ", letraoNegritaMediana);

                Chunk Dos17 = new Chunk("DE ACUERDO A LA SOLICITUD ", letraNormalMediana);
                Chunk Dos18 = new Chunk(_tableAnexo.Rows[0]["Solicitud"].ToString().ToUpper() + " " , letraoNegritaMediana);
                Chunk Dos19 = new Chunk("DE FECHA ", letraNormalMediana);
                Chunk Dos20 = new Chunk(fechaInicio.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + " ", letraoNegritaMediana);
                Chunk Dos21 = new Chunk("Y AUTORIZADA EN OFICIO ", letraNormalMediana);
                Chunk Dos22 = new Chunk(_tableAnexo.Rows[0]["Folio"].ToString().ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos23 = new Chunk("DE ", letraNormalMediana);
                Chunk Dos24 = new Chunk(fechaSolicitud.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + ", ", letraoNegritaMediana);
                Chunk Dos25 = new Chunk("POR LA ", letraNormalMediana);
                Chunk Dos26 = new Chunk("GERENCIA DE MANTENIMIENTO Y MODERNIZACIÓN DE EQUIPOS DE PEAJE; ", letraoNegritaMediana);
                Chunk Dos27 = new Chunk("PARA CUYO EFECTÓ FUÉ NECESARIO REPONER EN FECHA ", letraNormalMediana);
                Chunk Dos28 = new Chunk(fechaApertura.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + " ", letraoNegritaMediana);
                Chunk Dos29 = new Chunk("LAS PARTES QUE A CONTINUACIÓN SE DETALLAN.", letraNormalMediana);
                var parrafoDos = new Paragraph();
                parrafoDos.SetLeading(0, 1.4f);

                parrafoDos.Add(Dos1);
                parrafoDos.Add(Dos2);
                parrafoDos.Add(Dos3);
                parrafoDos.Add(Dos4);
                parrafoDos.Add(Dos5);
                parrafoDos.Add(Dos6);
                parrafoDos.Add(Dos7);
                parrafoDos.Add(Dos8);
                parrafoDos.Add(Dos9);
                parrafoDos.Add(Dos10);
                parrafoDos.Add(Dos11);
                parrafoDos.Add(Dos12);
                parrafoDos.Add(Dos13);
                parrafoDos.Add(Dos14);
                parrafoDos.Add(Dos15);
                parrafoDos.Add(Dos16);
                parrafoDos.Add(Dos17);
                parrafoDos.Add(Dos18);
                parrafoDos.Add(Dos19);
                parrafoDos.Add(Dos20);
                parrafoDos.Add(Dos21);
                parrafoDos.Add(Dos22);
                parrafoDos.Add(Dos23);
                parrafoDos.Add(Dos24);
                parrafoDos.Add(Dos25);
                parrafoDos.Add(Dos26);
                parrafoDos.Add(Dos27);
                parrafoDos.Add(Dos28);
                parrafoDos.Add(Dos29);
                parrafoDos.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Tres = new Chunk("LOS EQUIPOS/COMPONENTES DAÑADOS EL ADMINISTRADOR DEBERÁ IDENTIFICAR Y EMBALAR, ENVIANDOLOS EN UN PERÍODO DE 5 DÍAS MÁXIMO AL ÁLMACÉN DE LA COORDINACION REGIONAL POR OFICIO, PARA SU DESTINO FINAL CONFORME LA NORMA CAPUFE.", letraNormalMediana);
                var parrafoTres = new Paragraph();
                parrafoTres.SetLeading(0, 1.4f);
                parrafoTres.Add(Tres);
                parrafoTres.Alignment = Element.ALIGN_JUSTIFIED;

                Chunk Cuatro = new Chunk("LOS TRABAJOS, INSTALACIÓN Y OPERACIÓN DE LA PLAZA DE COBRO QUEDAN A ", letraNormalMediana);
                Chunk Cuatro2 = new Chunk("ENTERA SATISFACCIÓN ", letraoNegritaMediana);
                Chunk Cuatro3 = new Chunk("DEL ADMINISTRADOR DE LA PLAZA DE COBRO Y/O ENCARGADO DE TURNO.", letraNormalMediana);
                var parrafoCuatro = new Paragraph();
                parrafoCuatro.SetLeading(0, 1.4f);
                parrafoCuatro.Add(Cuatro);
                parrafoCuatro.Add(Cuatro2);
                parrafoCuatro.Add(Cuatro3);
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

        private IElement TablaComponentesDañados(PdfWriter writer)
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 11.28f, 17.28f, 15.28f, 14.28f, 14.28f, 12f, 15.56f }) { WidthPercentage = 100f };

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


                ////Agrupa los componentes por "Componente" y lleva la cuenta de cada componente
                var componentesGropuped = from s in _tableComponentesDañados.AsEnumerable()
                                        group s by s.Field<string>("Componente")
                                            into grp
                                            orderby grp.Key
                                            select new
                                            {
                                                grpComponente = grp.Key,
                                                grpMOD_MARCA = string.Join("\n\n", grp.Select(a => a["MOD/MARCA"])),
                                                grpInventario = string.Join("\n\n", grp.Select(a => a["Inventario"])),
                                                grpSerie = string.Join("\n\n", grp.Select(a => a["Serie"])),
                                                grpCarril = string.Join("\n\n", grp.Select(a => a["Carril"])),
                                                grpCount = grp.Count()
                                            };

                string NoSerie = "Sin Número";
                for (int i = 0; i < componentesGropuped.Count(); i++)
                {
                    if (!DBNull.Value.Equals(componentesGropuped.ElementAt(i).grpSerie) || componentesGropuped.ElementAt(i).grpSerie.Equals("S / N"))
                        NoSerie = componentesGropuped.ElementAt(i).grpSerie.ToString();

                    var cantidad = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpCount.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var componente = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpComponente.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var marcaMod = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpMOD_MARCA.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var serie = new PdfPCell(new Phrase(NoSerie, letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var inventario = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpInventario.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var ubicacion = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpCarril.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var Observaciones = new PdfPCell(new Phrase("Dañado", letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };

                    table.AddCell(cantidad);
                    table.AddCell(componente);
                    table.AddCell(marcaMod);
                    table.AddCell(serie);
                    table.AddCell(inventario);
                    table.AddCell(ubicacion);
                    table.AddCell(Observaciones);
                
                }

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

        private IElement TablaComponentesNuevos(PdfWriter writer, Document doc, float PosicionEnY)
        {
            try
            {
                //Encabezado
                PdfPTable table = new PdfPTable(new float[] { 14.28f, 14.28f, 14.28f, 14.28f, 14.28f, 13f, 15.56f }) { WidthPercentage = 100f };

                table.SetTotalWidth(new float[] { 11.28f, 17.28f, 15.28f, 14.28f, 14.28f, 12f, 15.56f });
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
                var encabezadoUbicacion = new PdfPCell(new Phrase("Ubicacion", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 1, PaddingRight = 1, PaddingBottom = 7 };
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



                var componentesGropuped = from s in _tableComponentesNuevos.AsEnumerable()
                                        group s by s.Field<string>("Componente")
                                            into grp
                                            orderby grp.Key
                                            select new
                                            {
                                                grpComponente = grp.Key,
                                                grpMOD_MARCA = string.Join("\n\n", grp.Select(a => a["MOD/MARCA"])),
                                                grpInventario = string.Join("\n\n", grp.Select(a => a["Inventario"])),
                                                grpSerie = string.Join("\n\n", grp.Select(a => a["Serie"])),
                                                grpCarril = string.Join("\n\n", grp.Select(a => a["Carril"])),
                                                grpCount = grp.Count()
                                            };

                string NoSerie = "Sin Número";
                for (int i = 0; i < componentesGropuped.Count(); i++)
                {
                    if (!DBNull.Value.Equals(componentesGropuped.ElementAt(i).grpSerie))
                        NoSerie = componentesGropuped.ElementAt(i).grpSerie.ToString();

                    var cantidad = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpCount.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var componente = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpComponente.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var marcaMod = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpMOD_MARCA.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var serie = new PdfPCell(new Phrase(NoSerie, letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var inventario = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpInventario.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var ubicacion = new PdfPCell(new Phrase(componentesGropuped.ElementAt(i).grpCarril.ToString(), letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };
                    var Observaciones = new PdfPCell(new Phrase("Nuevo", letraNormalMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 9, PaddingLeft = 3, PaddingRight = 3, PaddingBottom = 7 };

                    table.AddCell(cantidad);
                    table.AddCell(componente);
                    table.AddCell(marcaMod);
                    table.AddCell(serie);
                    table.AddCell(inventario);
                    table.AddCell(ubicacion);
                    table.AddCell(Observaciones);
                }

                var height = table.CalculateHeights(false);

                if (height > 1000)
                    height = height - 1000;

                if (height > PosicionEnY + 200)
                    doc.NewPage();

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

                Chunk Uno = new Chunk("Se cierra la presente acta en fecha ", letraoNegritaMediana);
                Chunk Uno2 = new Chunk(fechaApertura.ToString("d DE MMMMM DE yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + " siendo las ", letraoNegritaMediana);
                //Chunk Uno2 = new Chunk("20 de diciembre de 2021 siendo las ", letraoNegritaMediana);
                Chunk Uno3 = new Chunk(fechaCierre.ToString("hh:mm", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper() + " hrs.", letraoNegritaMediana);
                //Chunk Uno3 = new Chunk("18:00 hrs.", letraoNegritaMediana);
                var parrafoUno = new Paragraph();
                parrafoUno.SetLeading(0, 1.4f);
                parrafoUno.Add(Uno);
                parrafoUno.Add(Uno2);
                parrafoUno.Add(Uno3);
                parrafoUno.Alignment = Element.ALIGN_JUSTIFIED;

                var colParrafoUno = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 0,
                    PaddingBottom = 10
                };
                colParrafoUno.AddElement(parrafoUno);

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

                Chunk admin = _tableAnexo.Rows[0]["DelegationId"].Equals(2) || _tableAnexo.Rows[0]["DelegationId"].Equals(3) ? new Chunk("SUPERINTENDENTE OPERACIÓN / SUPERVISIÓN", letraoNegritaMediana) : new Chunk("ADMINISTRADOR Y ENCARGADO DE LA SUPERINTENDENCIA DE OPERACIÓN", letraoNegritaMediana);
                var parrafoAdmin = new Paragraph();
                parrafoAdmin.SetLeading(0, 1.2f);
                parrafoAdmin.Add(admin);
                parrafoAdmin.Alignment = Element.ALIGN_CENTER;

                Chunk subg = new Chunk("SUBGERENTE DE OPERACIÓN REGIONAL", letraoNegritaMediana);
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

                Chunk nomCapufe = new Chunk(_tableAnexo.Rows[0]["Admin"].ToString().ToUpper() + "\n Administrador o Encargado de Turno\n AUTORIZÓ", letraoNegritaMediana);
                var parrafonomCapufe = new Paragraph();
                parrafonomCapufe.SetLeading(0, 1.8f);
                parrafonomCapufe.Add(nomCapufe);
                parrafonomCapufe.Alignment = Element.ALIGN_CENTER;

                Chunk nomEmpresa = new Chunk(_tableAnexo.Rows[0]["Supervisor"].ToString().ToUpper() + "\n Técnico\n REALIZÓ", letraoNegritaMediana);
                var parrafonomEmpresa = new Paragraph();
                parrafonomEmpresa.SetLeading(0, 1.8f);
                parrafonomEmpresa.Add(nomEmpresa);
                parrafonomEmpresa.Alignment = Element.ALIGN_CENTER;

                string supervision = "";

                switch (_tableAnexo.Rows[0]["DelegationId"])
                {
                    case 1:
                        supervision = "C. CARLOS BÁRCENAS ORTEGA";
                        break;

                    case 2:
                        supervision = "ING. MARTHA ANGÉLICA MÁRQUEZ CUELLAR";
                        break;

                    case 3:
                        supervision = "LIC. ANTONIO ESTEBAN SÁNCHEZ CRUZ";
                        break;

                    default:
                        break;
                }
                Chunk nomAdministrador = _tableAnexo.Rows[0]["DelegationId"].Equals(3)? new Chunk(supervision + "\n Superintendente de Equipo de Control de Tránsito Unidad Regional Estado de México\n SUPERVISÓ", letraoNegritaMediana) : new Chunk(supervision + "\n SUPERVISÓ", letraoNegritaMediana);
                var parrafonomAdministrador = new Paragraph();
                parrafonomAdministrador.SetLeading(0, 1.8f);
                parrafonomAdministrador.Add(nomAdministrador);
                parrafonomAdministrador.Alignment = Element.ALIGN_CENTER;

                Chunk nomSubg = _tableAnexo.Rows[0]["DelegationId"].Equals(3) ? new Chunk(_tableAnexo.Rows[0]["Vo.Bo."].ToString().ToUpper() + "\n SUBGERENTE DE OPERACIÓN DE LA UNIDAD REGIONAL DE ESTADO DE MÉXICO \n Vo.Bo", letraoNegritaMediana) : new Chunk(_tableAnexo.Rows[0]["Vo.Bo."].ToString().ToUpper() + "\n Vo.Bo.", letraoNegritaMediana);
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

                //tablaCuatroFirmas.AddCell(espacioVacio);

                PdfPTable tablaDosFirmas = new PdfPTable(new float[] { 13.33f, 30f, 13.33f, 30f, 13.33f }) { WidthPercentage = 100f };

                /////////////////////////////////////////////////testigos/////////////////////////////////////////////

                Chunk nomEncargadoUno = new Chunk(_tableAnexo.Rows[0]["Testigo Uno"].ToString().ToUpper() + "\n Encargado de turno", letraoNegritaMediana);
                var parrafonomEncargadoUno = new Paragraph();
                parrafonomEncargadoUno.SetLeading(0, 1.8f);
                parrafonomEncargadoUno.Add(nomEncargadoUno);
                parrafonomEncargadoUno.Alignment = Element.ALIGN_CENTER;

                Chunk nomEncargadoDos = new Chunk(_tableAnexo.Rows[0]["Testigo Dos"].ToString().ToUpper() + "\n Encargado de turno", letraoNegritaMediana);
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

                //tablaDosFirmas.AddCell(espacioVacio2);

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
                    PaddingTop = 20,
                    PaddingBottom = 20,
                    Colspan = 5

                };

                var colParrafoUno = new PdfPCell()
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 12,
                    PaddingBottom = 18,
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

        public string appendRows(DataTable table, string column)
        {
            string list = "";
            int numList = 0;

            foreach (DataRow item in table.Rows)
            {
                if (table.Rows.IndexOf(item) == table.Rows.Count - 1)
                {
                    if (!list.Contains(item[column].ToString()))
                    {
                        if (!DBNull.Value.Equals(item[column]))
                            list += item[column];
                        else
                            list += "Sin Numero";
                        numList += 1;
                    }
                    else if (item[column].ToString().Equals(""))
                    {
                        list += "Sin Numero";
                        numList += 1;
                    }
                    else
                    {
                        list = list.Remove(list.Length - 2);
                    }
                }
                else
                {
                    if (!list.Contains(item[column].ToString()))
                    {
                        if (!DBNull.Value.Equals(item[column]))
                            list += item[column] + ", ";
                        else
                            list += "Sin Numero, ";

                        numList += 1;
                    }
                }

                if ( (numList > 1) && (table.Rows.IndexOf(item) == table.Rows.Count - 1))
                {
                    list = list.Remove(list.Length - item[column].ToString().Length - 2, 1);
                    list = list.Insert(list.Length - item[column].ToString().Length -1, " Y ");
                }
            }

            return list;
        }
        #endregion

    }
}
