
namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    public class AgregarObservaciones
    {
        #region Attributes
        private readonly DataTable _tableHeader;

        private readonly ApiLogger _apiLogger;

        private readonly string _clavePlaza;

        private readonly string _Mensaje;
        #endregion

        #region BaseFont
        public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion

        #region iText.Font
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        #endregion
        public AgregarObservaciones(ApiLogger apiLogger, DataTable tableHeader, string Mensaje, string clavePlaza)
        {
            _apiLogger = apiLogger;
            _tableHeader = tableHeader;
            _clavePlaza = clavePlaza;
            _Mensaje = Mensaje;
        }

        #region Metodos
        public PdfPTable TablaObservaciones()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                table.TotalWidth = 550f;


                var colTextoObservaciones = new PdfPCell(new Phrase(_Mensaje, letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 1, Colspan = 2 };

                table.AddCell(colTextoObservaciones);
                CeldasVacias(14, table);

                var celdaObservaciones = SeparacionObservaciones(Convert.ToString(_tableHeader.Rows[0]["Observaciones"]));
                int celdasTotalesObservaciones = 0;
                foreach (var linea in celdaObservaciones)
                {
                    if (linea.Contains('\n'))
                    {
                        string[] Divisiones = linea.Split('\n');
                        celdasTotalesObservaciones += Divisiones.Length;
                        foreach (var item in Divisiones)
                        {
                            var celdaLinea1 = new PdfPCell(new Phrase(Convert.ToString(item), letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                            table.AddCell(celdaLinea1);
                        }
                    }
                    else
                    {
                        celdasTotalesObservaciones += 1;
                        var celdaLinea = new PdfPCell(new Phrase(Convert.ToString(linea), letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                        table.AddCell(celdaLinea);
                    }
                }
                for (int i = 0; i < 3 - celdasTotalesObservaciones; i++)
                {
                    var celdaLinea = new PdfPCell(new Phrase("", letraNormalMediana)) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 1, FixedHeight = 15, HorizontalAlignment = Element.ALIGN_JUSTIFIED, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 8 };
                    table.AddCell(celdaLinea);
                }
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

        private List<string> SeparacionObservaciones(string observaciones)
        {
            List<string> lineaObservaciones = new List<string>();
            if (observaciones.Length <= 85)
            {
                lineaObservaciones.Add(observaciones);
                return lineaObservaciones;
            }

            string linea = string.Empty;
            for (int i = 0; i < observaciones.Length; i++)
            {
                if (observaciones[i].Equals(',') || observaciones[i].Equals('.') || observaciones[i].Equals('.') || observaciones[i].Equals(':'))
                {
                    if (i < observaciones.Length - 1 && !observaciones[i + 1].Equals(' '))
                    {
                        linea += $"{observaciones[i]} ";
                        continue;
                    }
                }
                linea += observaciones[i];
                if (linea.Length >= 85 && observaciones[i].Equals(' '))
                {
                    lineaObservaciones.Add(linea);
                    linea = string.Empty;
                }
            }
            lineaObservaciones.Add(linea);

            /*char[] separadores = new char[]{
                ' ',
                ',',
                '.'
            };
            var palabras = observaciones.Split(separadores);
            List<string> palabrasSinVacio = new List<string>();
            foreach(var palabra in palabras)
            {
                if(!string.IsNullOrEmpty(palabra))
                    palabrasSinVacio.Add(palabra);
            }
            string linea = string.Empty;
            foreach (var palabra in palabrasSinVacio)
            {
                if(!string.IsNullOrEmpty(palabra))
                {
                    linea += $"{palabra} ";
                    if(linea.Length > 100)
                    {
                        lineaObservaciones.Add(linea);
                        linea = string.Empty;
                    }
                    if(palabra == palabrasSinVacio[palabrasSinVacio.Count - 1] && linea.Length < 100)
                    {
                        lineaObservaciones.Add(linea);
                        linea = string.Empty;
                    }
                }
            }*/
            return lineaObservaciones;
        }

        public void CeldasVacias(int numeroCeldas, PdfPTable table)
        {
            for (int i = 0; i < numeroCeldas; i++)
                table.AddCell(new PdfPCell() { Border = 0 });
        }
    }
    #endregion
}
