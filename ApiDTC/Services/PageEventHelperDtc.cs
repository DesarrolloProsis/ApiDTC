using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Services
{
    public class PageEventHelperDtc : PdfPageEventHelper
    {
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
        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 15f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 7f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaDatos = new iTextSharp.text.Font(NegritaMediana, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChicaEncabezado = new iTextSharp.text.Font(NegritaChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 5f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalGrande = new iTextSharp.text.Font(NormalGrande, 15f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMedianaSub = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.UNDERLINE, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraSubAzulChica = new iTextSharp.text.Font(NormalChicaSubAzul, 5f, iTextSharp.text.Font.UNDERLINE, BaseColor.Blue);
        public static iTextSharp.text.Font letritasMiniMini = new iTextSharp.text.Font(fuenteLetrita, 1f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion
        #region Logo
        #endregion
        #endregion

        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;

        DataTable _tableHeader;

        DataTable _tableDTCData;
        ConvenioData _newConvenio;
        string _refNum;

        #region Properties
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _HeaderLeft;
        public string HeaderLeft
        {
            get { return _HeaderLeft; }
            set { _HeaderLeft = value; }
        }
        private string _HeaderRight;
        public string HeaderRight
        {
            get { return _HeaderRight; }
            set { _HeaderRight = value; }
        }
        private Font _HeaderFont;
        public Font HeaderFont
        {
            get { return _HeaderFont; }
            set { _HeaderFont = value; }
        }
        private Font _FooterFont;
        public Font FooterFont
        {
            get { return _FooterFont; }
            set { _FooterFont = value; }
        }
        #endregion
        // we override the onOpenDocument method

        public PageEventHelperDtc(DataTable tableHeader, DataTable talbleData, string refNum)
        {
            this._tableDTCData = talbleData;
            this._tableHeader = tableHeader;
            this._refNum = refNum;
            ConvenioHelper convenioHelper = new ConvenioHelper();
            this._newConvenio = convenioHelper.GetConvenioData(tableHeader, talbleData);
        }
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(document.PageSize.Width, 50);
            }
            catch (DocumentException ex)
            {
                var apiLogger = new ApiLogger();
                apiLogger.WriteLog("PEH", ex, "PageEventHelper: OnOpenDocument", 7);
            }
            catch (System.IO.IOException ex)
            {
                var apiLogger = new ApiLogger();
                apiLogger.WriteLog("PEH", ex, "PageEventHelper: OnOpenDocument", 2);
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Rectangle pageSize = document.PageSize;
            if (Title != string.Empty)
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 15);
                cb.SetRgbColorFill(50, 50, 200);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
                cb.ShowText(Title);
                cb.EndText();
            }
            document.Add(tablaEncabezado());
            document.Add(tablaSiniestro());
            document.Add(tablaSiniestroMore());
            if (HeaderLeft + HeaderRight != string.Empty)
            {
                PdfPTable HeaderTable = new PdfPTable(2);
                HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                HeaderTable.TotalWidth = pageSize.Width - 80;
                HeaderTable.SetWidthPercentage(new float[] { 45, 45 }, pageSize);

                PdfPCell HeaderLeftCell = new PdfPCell(new Phrase(8, HeaderLeft, HeaderFont))
                {
                    Padding = 5,
                    PaddingBottom = 8,
                    BorderWidthRight = 0
                };
                HeaderTable.AddCell(HeaderLeftCell);
                PdfPCell HeaderRightCell = new PdfPCell(new Phrase(8, HeaderRight, HeaderFont))
                {
                    HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                    Padding = 5,
                    PaddingBottom = 8,
                    BorderWidthLeft = 0
                };
                HeaderTable.AddCell(HeaderRightCell);
                cb.SetRgbColorFill(0, 0, 0);
                HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), cb);
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Page " + pageN + "/";
            float len = bf.GetWidthPoint(text, 8);
            Rectangle pageSize = document.PageSize;
            cb.SetRgbColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(20));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(20));
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                "PROYECTOS Y SISTEMAS INFORMATICOS, S.A DE C.V PALENQUE #489 COL.VERTIZ NARVARTE C.P 03600 BENITO JUAREZ CDMX TEL. 5552437267",
                pageSize.GetRight(700),
                pageSize.GetBottom(20), 0);
            cb.EndText();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }

        private IElement tablaFinal(int op)
        {
            var tablaFinal = new PdfPTable(new float[] { 40f, 7f, 34f, 7f, 26f }) { WidthPercentage = 100f };

            
            var innerTable = new PdfPTable(1);
            var colAutorizacion = new PdfPCell(new Phrase("AUTORIZACIÓN TÉCNICA Y COMERCIAL", letraNormalChica)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 0, Padding = 2 };
            PdfPCell colFirma;
            if(op != 0)
            {
                iTextSharp.text.Image firma = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\firma.png");
                firma.ScaleAbsolute(35f, 40f);
                colFirma = new PdfPCell(firma) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 0, Padding = 2 };
            }
            else
                colFirma = new PdfPCell(new Phrase("", letraNormalChica)) { FixedHeight = 20f, HorizontalAlignment = Element.ALIGN_CENTER, Border = 0, Padding = 2 };
            var colNombreDirector = new PdfPCell(new Phrase("Autorización Comercial Director Comercial\nC.P Hermilia Guzman Añorve", letraNormalChica)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 0, Padding = 2 };
            innerTable.AddCell(colAutorizacion);
            innerTable.AddCell(colFirma);
            innerTable.AddCell(colNombreDirector);

            var colEmpy4 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
            colEmpy4.Border = 0;

            var colEmpy5 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_CENTER };
            colEmpy5.Border = 0;

            var colEmpy6 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
            colEmpy6.Border = 0;

            var administradorLine = new Chunk(Convert.ToString(_tableHeader.Rows[0]["AdminName"]), letraNormalMedianaSub);
            var administrador = new Chunk("Administrador Plaza de Cobro\n" + Convert.ToString(_tableHeader.Rows[0]["AdminMail"]), letraNormalMediana);

            var colAdministrador = new PdfPCell(new Phrase(administradorLine)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM };            
            colAdministrador.Border = 0;


            var colEmpy7 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
            colEmpy7.Border = 0;

            var colEmpy8 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_CENTER };
            colEmpy8.Border = 0;

            var colEmpy9 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
            colEmpy9.Border = 0;

            var colEmpy10 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
            colEmpy10.Border = 0;

            var colAdministradorSinLinea = new PdfPCell(new Phrase(administrador)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM };
            colAdministradorSinLinea.Border = 0;

            var tablaInterna = new PdfPCell();
            tablaInterna.AddElement(innerTable);

            tablaFinal.AddCell(tablaInterna);
            tablaFinal.AddCell(colEmpy4);
            tablaFinal.AddCell(colEmpy5);
            tablaFinal.AddCell(colEmpy6);
            tablaFinal.AddCell(colAdministrador);
            tablaFinal.AddCell(colEmpy7);
            tablaFinal.AddCell(colEmpy8);
            tablaFinal.AddCell(colEmpy9);
            tablaFinal.AddCell(colEmpy10);
            tablaFinal.AddCell(colAdministradorSinLinea);
            return tablaFinal;
        }

        private IElement tablaEncabezado()
        {
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\prosis-logo.jpg");
            logo.ScalePercent(10f);
            
            //Encabezado
            var tablaEncabezado = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100f };
            var col1 = new PdfPCell(logo) { Border = 0 };
            var col2 = new PdfPCell(new Phrase("DICTAMEN TÉCNICO Y COTIZACIÓN", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 10, PaddingRight = 20, PaddingLeft = 20 };
            //Creamos Chunk Para dosTipo deLetra en un Parrafo
            var referencia = new Chunk("Referencia:    ", letraNormalMediana);
            var numreferencia = new Chunk(_refNum, letraoNegritaMediana);
            var refCompleta = new Phrase(referencia);
            refCompleta.Add(numreferencia);
            var col3 = new PdfPCell(refCompleta) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE };
            tablaEncabezado.AddCell(col1);
            tablaEncabezado.AddCell(col2);
            tablaEncabezado.AddCell(col3);
            var colVacia = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            tablaEncabezado.AddCell(colVacia);
            tablaEncabezado.AddCell(colVacia);
            tablaEncabezado.AddCell(colVacia);
            tablaEncabezado.AddCell(colVacia);
            tablaEncabezado.AddCell(colVacia);
            tablaEncabezado.AddCell(colVacia);
            return tablaEncabezado;
        }

        private IElement tablaSiniestro()
        {
            var tablaSiniestro = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100f };
            //Agregamos Chunk Para 2 letras
            string labelConvenioOferta = "Contrato/Oferta:";
            if (_newConvenio.ConvenioNuevo)
            {
                labelConvenioOferta = "Contrato/Convenio:";
            }
            var contratoOferta = new Chunk(labelConvenioOferta, letraNormalChica);
            var numContratoOferta = new Chunk(Convert.ToString(Convert.ToString(_tableHeader.Rows[0]["Agrement"])), letraoNegritaChica);
            //var numContratoOferta = new Chunk(Convert.ToString(_newConvenio.Agremment), letraoNegritaChica);

            var ContratoOfertaCompleto = new Phrase(contratoOferta);
            ContratoOfertaCompleto.Add(numContratoOferta);
            var col4 = new PdfPCell(new Phrase(ContratoOfertaCompleto)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
            //-----------------------------------------
            var col5 = new PdfPCell(new Phrase("EN CASO DE SINIESTRO", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 3};
            //-----------------------------------------
            //Agregamos Chunk 2 letras
            var tipoDictamen = new Chunk("Tipo de Dictamen        ", letraNormalChica);
            var dictamen = new Chunk("CORRECTIVO", letraoNegritaChica);
            var tipoDictamenCompleto = new Phrase(tipoDictamen);
            tipoDictamenCompleto.Add(dictamen);
            var col6 = new PdfPCell(new Phrase(tipoDictamenCompleto)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE };

            tablaSiniestro.AddCell(col4);
            tablaSiniestro.AddCell(col5);
            tablaSiniestro.AddCell(col6);
            return tablaSiniestro;
        }

         private IElement tablaSiniestroMore()
        {
            var tablaSiniestroMore = new PdfPTable(new float[] { 15f, 80f, 20f, 40f, 15f, 65f, 35f, 40f }) { WidthPercentage = 100f };

            var col1 = new PdfPCell(new Phrase("Atención:", letraNormalChica)) { Border = 0 };
            var col2 = new PdfPCell(new Phrase(Convert.ToString(_tableHeader.Rows[0]["ManagerName"]), letraNormalChica)) { Border = 0 };
            //var col2 = new PdfPCell(new Phrase(Convert.ToString(_newConvenio.Manager), letraNormalChica)) { Border = 0 };

            var col3 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col4 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col5 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col6 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col7 = new PdfPCell(new Phrase("Descripción:", letraNormalChica)) { Border = 0 };
            var col8 = new PdfPCell(new Phrase(Convert.ToString(_tableDTCData.Rows[0]["TipoDescripicon"]), letraoNegritaChicaEncabezado)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 };

            var col9 = new PdfPCell(new Phrase("Cargo:", letraNormalChica)) { Border = 0 };
            var col10 = new PdfPCell(new Phrase(Convert.ToString(_tableHeader.Rows[0]["Position"]), letraNormalChica)) { Border = 0 };
            //var col10 = new PdfPCell(new Phrase(_newConvenio.Cargo, letraNormalChica)) { Border = 0 };

            var col11 = new PdfPCell(new Phrase("No. Siniestro", letraNormalChica)) { Border = 0 };
            string NSiniestro = "";
            if(Convert.ToString(_tableDTCData.Rows[0]["SinisterNumber"]).Equals("") || Convert.ToString(_tableDTCData.Rows[0]["SinisterNumber"]).Equals(null))
            {
                NSiniestro = "SIN NÚMERO DE SINIESTRO";
            }
            else
            {
                NSiniestro = Convert.ToString(_tableDTCData.Rows[0]["SinisterNumber"]);
            }
            var col12 = new PdfPCell(new Phrase(NSiniestro, letraoNegritaChicaEncabezado)) { Border = 0 };

            var col13 = new PdfPCell(new Phrase("No. Reporte", letraNormalChica)) { Border = 0 };
            var col14 = new PdfPCell(new Phrase(Convert.ToString(_tableDTCData.Rows[0]["ReportNumber"]), letraoNegritaChicaEncabezado)) { Border = 0 };

            var col15 = new PdfPCell(new Phrase("Lugar de fecha de envío:", letraNormalChica)) { Border = 0 };
            var col16 = new PdfPCell(new Phrase("Ciudad de México a " + Convert.ToDateTime(_tableDTCData.Rows[0]["ElaborationDate"]).ToString("dd/MM/yyyy"), letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 };


            var col17 = new PdfPCell(new Phrase("Correo:", letraNormalChica)) { Border = 0 };
            var col18 = new PdfPCell(new Phrase(Convert.ToString(_tableHeader.Rows[0]["Mail"]), letraSubAzulChica)) { Border = 0 };
            //var col18 = new PdfPCell(new Phrase(_newConvenio.Email, letraSubAzulChica)) { Border = 0 };

            var col19 = new PdfPCell(new Phrase("Fecha de siniestro:", letraNormalChica)) { Border = 0 };
            var col20 = new PdfPCell(new Phrase(Convert.ToDateTime(_tableDTCData.Rows[0]["SinisterDate"]).ToString("dd/MM/yyyy"), letraoNegritaDatos)) { Border = 0 };

            var col21 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col22 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col23 = new PdfPCell(new Phrase("Técnico Responsable:", letraNormalChica)) { Border = 0 };
            var col24 = new PdfPCell(new Phrase(Convert.ToString(_tableDTCData.Rows[0]["TecnicoResponsable"]), letraoNegritaChicaEncabezado)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 };

            var col25 = new PdfPCell(new Phrase("Plaza de cobro:", letraNormalChica)) { Border = 0 };
            var col26 = new PdfPCell(new Phrase(Convert.ToString(_tableHeader.Rows[0]["Plaza"]), letraNormalChica)) { Border = 0 };

            var col27 = new PdfPCell(new Phrase("Folio(s) Fallas(s)", letraNormalChica)) { Border = 0 };
            var col28 = new PdfPCell(new Phrase(Convert.ToString(_tableDTCData.Rows[0]["FailureNumber"]), letraNormalChica)) { Border = 0 };


            var col29 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col30 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            string labelCordinacion = "Unidad Regional";
            if (_newConvenio.ConvenioNuevo) 
            {
                labelCordinacion = "Unidad Regional";
            }

            var col31 = new PdfPCell(new Phrase(labelCordinacion, letraNormalChica)) { Border = 0 };
            var col32 = new PdfPCell(new Phrase(Convert.ToString(_tableHeader.Rows[0]["RegionalCoordination"]), letraoNegritaChicaEncabezado)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 };
            //var col32 = new PdfPCell(new Phrase(_newConvenio.Cordinacion, letraoNegritaChicaEncabezado)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 };

            var col33 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col34 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col35 = new PdfPCell(new Phrase("Fecha de falla:", letraNormalChica)) { Border = 0 };
            var col36 = new PdfPCell(new Phrase(Convert.ToDateTime(_tableDTCData.Rows[0]["FailureDate"]).ToString("dd/MM/yyyy"), letraNormalChica)) { Border = 0 };

            var col37 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col38 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col39 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col40 = new PdfPCell(new Phrase("", letraoNegritaChicaEncabezado)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 };

            var col41 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col42 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col43 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col44 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col45 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            var col46 = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };

            var col47 = new PdfPCell(new Phrase("Fecha de Elaboración:", letraNormalChica)) { Border = 0 };
            var col48    = new PdfPCell(new Phrase(Convert.ToDateTime(_tableDTCData.Rows[0]["ElaborationDate"]).ToString("dd/MM/yyyy"), letraoNegritaChicaEncabezado)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 };

            tablaSiniestroMore.AddCell(col1);
            tablaSiniestroMore.AddCell(col2);
            tablaSiniestroMore.AddCell(col3);
            tablaSiniestroMore.AddCell(col4);
            tablaSiniestroMore.AddCell(col5);
            tablaSiniestroMore.AddCell(col6);
            tablaSiniestroMore.AddCell(col7);
            tablaSiniestroMore.AddCell(col8);
            tablaSiniestroMore.AddCell(col9);
            tablaSiniestroMore.AddCell(col10);
            tablaSiniestroMore.AddCell(col11);
            tablaSiniestroMore.AddCell(col12);
            tablaSiniestroMore.AddCell(col13);
            tablaSiniestroMore.AddCell(col14);
            tablaSiniestroMore.AddCell(col15);
            tablaSiniestroMore.AddCell(col16);
            tablaSiniestroMore.AddCell(col17);
            tablaSiniestroMore.AddCell(col18);
            tablaSiniestroMore.AddCell(col19);
            tablaSiniestroMore.AddCell(col20);
            tablaSiniestroMore.AddCell(col21);
            tablaSiniestroMore.AddCell(col22);
            tablaSiniestroMore.AddCell(col23);
            tablaSiniestroMore.AddCell(col24);
            tablaSiniestroMore.AddCell(col25);
            tablaSiniestroMore.AddCell(col26);
            tablaSiniestroMore.AddCell(col27);
            tablaSiniestroMore.AddCell(col28);
            tablaSiniestroMore.AddCell(col29);
            tablaSiniestroMore.AddCell(col30);
            tablaSiniestroMore.AddCell(col31);
            tablaSiniestroMore.AddCell(col32);
            tablaSiniestroMore.AddCell(col33);
            tablaSiniestroMore.AddCell(col34);
            tablaSiniestroMore.AddCell(col35);
            tablaSiniestroMore.AddCell(col36);
            tablaSiniestroMore.AddCell(col37);
            tablaSiniestroMore.AddCell(col38);
            tablaSiniestroMore.AddCell(col39);
            tablaSiniestroMore.AddCell(col40);
            tablaSiniestroMore.AddCell(col41);
            tablaSiniestroMore.AddCell(col42);
            tablaSiniestroMore.AddCell(col43);
            tablaSiniestroMore.AddCell(col44);
            tablaSiniestroMore.AddCell(col45);
            tablaSiniestroMore.AddCell(col46);
            tablaSiniestroMore.AddCell(col47);
            tablaSiniestroMore.AddCell(col48);
            var colvacia = new PdfPCell(new Phrase("", letraNormalChica)) { Border = 0 };
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            tablaSiniestroMore.AddCell(colvacia);
            return tablaSiniestroMore;
        }

    }
}
