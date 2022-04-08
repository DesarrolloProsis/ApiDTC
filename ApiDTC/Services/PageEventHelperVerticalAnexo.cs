using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;

namespace ApiDTC.Services
{
    public class PageEventHelperVerticalAnexo : PdfPageEventHelper
    {
        private readonly DataTable _table;
        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;
        //This select the type of document we are creating, A or B.
        public static string _tipo;
        //Why we are documenting in english??
        //Aahh, i get it, this is a template
        public PageEventHelperVerticalAnexo(string Tipo, DataTable table)
        {
            _tipo = Tipo;
            _table = table;
        }

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

        #region BaseFont

        public static string path = $@"{System.Environment.CurrentDirectory}\Media\Montserrat\";

        public static BaseFont NegritaGrande = BaseFont.CreateFont(path + "Montserrat Bold 700.ttf", BaseFont.WINANSI, true);
        //public static BaseFont NegritaGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteMini = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion
        #region iText.Font
        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 10f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 9f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNormalChicaFirmas = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        #endregion

        // we override the onOpenDocument method

        public void CeldasVacias(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell() { Border = 0, PaddingTop = 25, PaddingBottom = 25
                });
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
                apiLogger.WriteLog("PHV", ex, "PageEventHelperVerticalAnexo: OnOpenDocument", 7);
            }
            catch (System.IO.IOException ex)
            {
                var apiLogger = new ApiLogger();
                apiLogger.WriteLog("PHV", ex, "PageEventHelperVerticalAnexo: OnOpenDocument", 2);
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Rectangle pageSize = document.PageSize;
            int pageN = writer.PageNumber;

            if (Title != string.Empty)
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 15);
                cb.SetRgbColorFill(50, 50, 200);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
                cb.ShowText(Title);
                cb.EndText();
            }

            PdfPTable table = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100f };

            iTextSharp.text.Image logo_encabezado = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\Encabezado Anexo.png");
            logo_encabezado.ScalePercent(8f);


            PdfPCell collogo_encabezado = new PdfPCell(logo_encabezado)
            {
                Border = 0,
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 10,
                PaddingTop = 5,
                PaddingBottom = 5
            };
            table.AddCell(collogo_encabezado);

            document.Add(table);

            if (pageN == 1)
            {
                PdfPTable tablaEncabezado = new PdfPTable(new float[] { 16.67f, 16.67f, 16.67f, 16.67f, 16.67f, 16.67f }) { WidthPercentage = 100f };


                var colFormato = new PdfPCell(new Phrase("FORMATO 1-" + _tipo, letraoNegritaGrande)) 
                { 
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT, 
                    VerticalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 12,
                    PaddingBottom = 12,
                    Colspan = 6 
                };
                var colActa = new PdfPCell(new Phrase("ACTA ADMINISTRATIVA INFORMATIVA (ENTREGA-RECEPCIÓN)", letraoNegritaGrande))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 0,
                    Colspan = 6
                };
                var colYear = new PdfPCell(new Phrase("2022", letraoNegritaGrande))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    Padding = 0,
                    Colspan = 6
                };
                tablaEncabezado.AddCell(colFormato);
                tablaEncabezado.AddCell(colActa);
                tablaEncabezado.AddCell(colYear);

                var colFecha = new PdfPCell(new Phrase(PrintTime.ToString(), letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                tablaEncabezado.AddCell(colFecha);


                document.Add(tablaEncabezado);
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Página. " + pageN;
            float len = bf.GetWidthPoint(text, 6.5f);
            Rectangle pageSize = document.PageSize;
            cb.SetRgbColorFill(192, 152, 82);

            cb.AddTemplate(template, pageSize.GetLeft(-30) + len, pageSize.GetBottom(20));
            cb.BeginText();
            cb.SetFontAndSize(bf, 8f);
            switch (_table.Rows[0]["DelegationId"])
            {
                case 1:
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                        "Calzada de los Reyes No. 24 Colonia Tetela del Monte C.P. 62130 Cuernavaca, Morelos.",
                        pageSize.GetRight(550),
                        pageSize.GetBottom(70), 0);
                    break;

                case 2:
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                        "Km. 7+100 Autopista Querétaro-Irapuato, Corregidora, Querétaro",
                        pageSize.GetRight(550),
                        pageSize.GetBottom(70), 0);
                    break;

                case 3:
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                        "Km. 5+000 Autopista Champa Lecheria, Col. Ejidos de San Miguel, Atizapan de Zaragoza, Edo. de México.",
                        pageSize.GetRight(550),
                        pageSize.GetBottom(70), 0);
                    break;

                default:
                    break;
            }
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(bf, 8f);

            switch (_table.Rows[0]["DelegationId"])
            {
                case 1:
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                        "Tel:  01 (777) 329 2100              www.gob.mx/capufe   " + text,
                        pageSize.GetRight(552),
                        pageSize.GetBottom(57), 0);
                    break;

                case 2:
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                        "Tel:  442 238 16 00                  www.gob.mx/capufe   " + text,
                        pageSize.GetRight(552),
                        pageSize.GetBottom(57), 0);
                    break;

                case 3:
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                        "Tel:  777 329 2100                   www.gob.mx/capufe   " + text,
                        pageSize.GetRight(552),
                        pageSize.GetBottom(57), 0);
                    break;

                default:
                    break;
            }

            cb.EndText();

            PdfPTable table = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f };

            iTextSharp.text.Image logo_footer = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\Pie de Pagina Ricardo Flores Magon.png");
            logo_footer.ScalePercent(12.1f);

            PdfPCell collogo_footer = new PdfPCell(logo_footer)
            {
                Border = 0,
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_MIDDLE,
                VerticalAlignment = Element.ALIGN_MIDDLE,
            };
            logo_footer.SetAbsolutePosition(document.PageSize.Width -560f, document.PageSize.Height -775f);
            document.Add(logo_footer);
            //table.AddCell(collogo_footer);
            //table.WriteSelectedRows(0, 5, 550, 80, writer.DirectContent);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(295, 37);
            template.ShowText("de " + (writer.PageNumber - 1));
            template.EndText();
        }
    }
}
