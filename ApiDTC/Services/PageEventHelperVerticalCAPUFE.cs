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
    public class PageEventHelperVerticalCAPUFE : PdfPageEventHelper
    {
        //private readonly string _carril;
        
        private readonly DataTable _Informacion;
        //private readonly DataTable _Administracion;
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

        #region BaseFont
        public static BaseFont NegritaGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NormalChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont fuenteMini = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion
        #region iText.Font
        public static iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 11f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 9f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraoNormalChicaFirmas = new iTextSharp.text.Font(NormalChica, 6f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

        public PageEventHelperVerticalCAPUFE(DataTable informacion)//, string carril)
        {
            //_Administracion = administracion;
            _Informacion = informacion;
          //  _carril = carril;

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

        public override void OnParagraph(PdfWriter writer, Document document, float paragraphPosition)
        {
            base.OnParagraph(writer, document, paragraphPosition);
            PdfPTable table4 = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100f };


            var colPlaza = new PdfPCell(new Phrase("OWO", letraNormalChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingRight = 5 };
            var colNombre = new PdfPCell(new Phrase("UWU", letraNormalChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 0, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingLeft = 5 };
            //var colCarril = new PdfPCell(new Phrase("Carril: B18 MULTIMODAL", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

            table4.AddCell(colPlaza);
            table4.AddCell(colNombre);
            //table4.AddCell(colCarril);
            document.Add(table4);

        }
        public override void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
        {
            base.OnChapter(writer, document, paragraphPosition, title);
        }

        public void CeldasVacias(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell() { Border = 0 });
        }
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Rectangle pageSize = document.PageSize;
            int pageN = writer.PageNumber;
            //Paragraph paragraph = new Paragraph("");
            Chapter chapter = new Chapter(0);

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

            iTextSharp.text.Image logo_capufe = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\logo-capufe.png");
            logo_capufe.ScalePercent(5f);

            iTextSharp.text.Image logo_comunicaciones = iTextSharp.text.Image.GetInstance($@"{System.Environment.CurrentDirectory}\Media\logo-comunicaciones.png");
            logo_comunicaciones.ScalePercent(20f);
            PdfPCell collogo_capufe = new PdfPCell(logo_comunicaciones)
            {
                Border = 0,
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            PdfPCell collogo_comunicaciones = new PdfPCell(logo_capufe)
            {
                Border = 0,
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(collogo_capufe);
            table.AddCell(collogo_comunicaciones);
            
            table.AddCell(new PdfPCell() { Border = 0 });
            document.Add(table);

//Encabezado primera pagina 

            if (pageN == 1)
            {
                PdfPTable tablaEncabezado = new PdfPTable(new float[] { 16.67f, 16.67f, 16.67f, 16.67f, 16.67f, 16.67f }) { WidthPercentage = 100f };



                var colTitulo = new PdfPCell(new Phrase("ANEXO 1.13 FORMATO PARA EL INFORME DE INVENTARIO DE EQUIPOS Y COMPONENTES DE PEAJE AL INICIAR LA VIGENCIA DE LOS SERVICIOS", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 6 };
                tablaEncabezado.AddCell(colTitulo);

                CeldasVacias(5, tablaEncabezado);
                var colFecha = new PdfPCell(new Phrase("Fecha: 01/01/2021", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                tablaEncabezado.AddCell(colFecha);

                
                document.Add(tablaEncabezado);
            }

            PdfPTable table4 = new PdfPTable(new float[] { 50f, 50f}) { WidthPercentage = 100f };

            
            var colPlaza = new PdfPCell(new Phrase("PLAZA DE COBRO: No. 004", letraNormalChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingRight =5 };
            var colNombre = new PdfPCell(new Phrase("Nombre: "+ _Informacion.Rows[0]["Nombre"], letraNormalChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 0, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, PaddingLeft = 5 };
            //var colCarril = new PdfPCell(new Phrase("Carril: "+_carril, letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
            table4.AddCell(colPlaza);
            table4.AddCell(colNombre);
            //table4.AddCell(colCarril);
            document.Add(table4);

            document.Add(chapter);
            //document.Add(paragraph);

            PdfPTable table2 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };

            var encabezadoDescripcion = new PdfPCell(new Phrase("Descripción", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
            //var encabezadoDetalle = new PdfPCell(new Phrase("Detalle", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
            var encabezadoMarcaModelo = new PdfPCell(new Phrase("Marca/Modelo", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
            //var encabezadoModelo = new PdfPCell(new Phrase("Modelo", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
            var encabezadoSerie = new PdfPCell(new Phrase("No. de Serie", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
            var encabezadoInventario = new PdfPCell(new Phrase("No. de Inventario", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
            //var encabezadoItem = new PdfPCell(new Phrase("ITEM", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
            var encabezadoObservaciones = new PdfPCell(new Phrase("Observaciones", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };


            //table2.AddCell(Equipamiento);
            table2.AddCell(encabezadoDescripcion);
            //table.AddCell(encabezadoDetalle);
            table2.AddCell(encabezadoMarcaModelo);
            //table.AddCell(encabezadoModelo);
            table2.AddCell(encabezadoSerie);
            table2.AddCell(encabezadoInventario);
            //table.AddCell(encabezadoItem);
            table2.AddCell(encabezadoObservaciones);

            table2.AddCell(new PdfPCell() { Border = 0 });
            document.Add(table2);
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
                "PROYECTOS Y SISTEMAS INFORMATICOS, S.A DE C.V PALENQUE #489 COL.VERTIZ NARVARTE C.P 03600 BENITO JUAREZ CDMX TEL. 5552437267",
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
