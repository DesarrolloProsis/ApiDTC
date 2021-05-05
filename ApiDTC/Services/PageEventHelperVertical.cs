using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Services
{
    public class PageEventHelperVertical : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;
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
                apiLogger.WriteLog("PHV", ex, "PageEventHelperVertical: OnOpenDocument", 7);
            }
            catch (System.IO.IOException ex)
            {
                var apiLogger = new ApiLogger();
                apiLogger.WriteLog("PHV", ex, "PageEventHelperVertical: OnOpenDocument", 2);
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

            PdfPTable table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\prosis-logo.jpg");
            logo.ScalePercent(10f);
            PdfPCell colLogo = new PdfPCell(logo) { 
                Border = 0, 
                Colspan = 4,
                HorizontalAlignment = Element.ALIGN_LEFT, 
                VerticalAlignment = Element.ALIGN_MIDDLE 
            };
            table.AddCell(colLogo);
            for (int i = 0; i < 7; i++)
                table.AddCell(new PdfPCell() { Border = 0 });
            document.Add(table);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Pág. " + pageN;
            float len = bf.GetWidthPoint(text, 6.5f);
            Rectangle pageSize = document.PageSize;
            cb.SetRgbColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 6.5f);
            cb.SetTextMatrix(pageSize.GetRight(40), pageSize.GetBottom(20));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(template, pageSize.GetLeft(-30) + len, pageSize.GetBottom(20));
            cb.BeginText();
            cb.SetFontAndSize(bf, 6.5f);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                "PROYECTOS Y SISTEMAS INFORMATICOS, S.A DE C.V AV.DOCTOR JOSE MARIA VERTIZ No.1238 INT.1 LETRAN VALLE C.P 03650 BENITO JUAREZ D.F TEL. 5552838256",
                pageSize.GetRight(580),
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
    }
}
