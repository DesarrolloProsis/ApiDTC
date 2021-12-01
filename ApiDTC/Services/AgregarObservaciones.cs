
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
        private readonly string _Contenido;

        private readonly ApiLogger _apiLogger;

        private readonly string _clavePlaza;

        private readonly string _MensajeEncabezado;

        private readonly string _strLog;

        private readonly int _numError1;

        private readonly int _numError2;
        #endregion

        #region BaseFont
        public static BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        public static BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
        #endregion

        #region iText.Font
        public static iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        public static iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 8f, iTextSharp.text.Font.BOLD, BaseColor.Black);
        #endregion
        public AgregarObservaciones(ApiLogger apiLogger, string Contenido, string MensajeEncabezado, string clavePlaza, string strLog, int numError1, int numError2)
        {
            _apiLogger = apiLogger;
            _Contenido = Contenido;
            _clavePlaza = clavePlaza;
            _MensajeEncabezado = MensajeEncabezado;
            _strLog = strLog;
            _numError1 = numError1;
            _numError2 = numError2;
        }

        #region Metodos
        public PdfPTable TablaObservaciones()
        {
            try
            {
                PdfPTable table = new PdfPTable(new float[] { 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f, 12.5f }) { WidthPercentage = 100f };
                table.TotalWidth = 550f;


                var colMensajeEncabezado = new PdfPCell(new Phrase(_MensajeEncabezado, letraoNegritaMediana)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 6 };

                table.AddCell(colMensajeEncabezado);
                CeldasVacias(10, table);

                var celdaObservaciones = SeparacionObservaciones(_Contenido);
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

                CeldasVacias(8, table);
                return table;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, _strLog, _numError1);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, _strLog, _numError2);
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
