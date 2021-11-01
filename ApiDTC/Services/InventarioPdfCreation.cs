

namespace ApiDTC.Services
{
    using ApiDTC.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    public class InventarioPdfCreation
    {
        #region Attributes
        private readonly DataTable _Informacion;

        private readonly DataTable _Cuerpo;

        private readonly string _clavePlaza;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Pdf Configuration
        //Tipo de Letras 
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
        #endregion
        #region Logo
        #endregion
        #endregion

        #region Constructors

        public InventarioPdfCreation(string clavePlaza,DataTable Informacion, DataTable Cuerpo, ApiLogger apiLogger)
        {
            _Informacion = Informacion;
            _Cuerpo = Cuerpo;
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
        }

        #endregion
        #region Methods
        
        public Response NewPdf(string folder)
        {
            string directory, filename, path;
            directory = $@"{folder}\{_clavePlaza.ToUpper()}\Almacén";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                return new Response
                {
                    Message = "Error: No existe el directorio",
                    Result = null
                };
            }

            filename = $"Reporte Almacén.pdf";
            
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
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: NewPdf", 2);
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
                    doc.SetMargins(30f, 30f, 30f, 30f);
                    doc.AddAuthor("PROSIS");
                    doc.AddTitle("ANEXO 1.13 FORMATO PARA EL INFORME DE INVENTARIO DE EQUIPOS Y COMPONENTES DE PEAJE AL INICIAR LA VIGENCIA DE LOS SERVICIOS");
                    

                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelperVerticalCAPUFE(_Informacion, "Plaza");                
                    writer.Open();
                    doc.Open();
                   
                    tableSort(_Cuerpo, doc);
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
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: NewPdf", 2);
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

        private IElement tableSort(DataTable Equipamiento, Document doc)
        {

            PdfPTable tablaCuerpo_1 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpo_2 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpo_3 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpo_5 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpo_6 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpo_7 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpoVacio = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };

            var tablaCarril = new DataTable();
            tablaCarril = Equipamiento.Clone();

            foreach (DataRow carril in _Informacion.Rows)
            {
                foreach (DataRow Fila in Equipamiento.Rows)
                {
                    if(Equals(carril["Lane"], Fila["Carril"]))
                        tablaCarril.ImportRow(Fila);
                }

                var tablaEquipamientoEncabezado_1 = new PdfPCell(new Phrase("Equipamiento de Carril", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.Yellow, Colspan = 5 };
                tablaCuerpo_1.AddCell(tablaEquipamientoEncabezado_1);

                var tablaEquipamientoEncabezado_2 = new PdfPCell(new Phrase("Equipamiento de Cabina", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.Yellow, Colspan = 5 };
                tablaCuerpo_2.AddCell(tablaEquipamientoEncabezado_2);

                var tablaEquipamientoEncabezado_3 = new PdfPCell(new Phrase("Equipamiento de Piso", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.Yellow, Colspan = 5 };
                tablaCuerpo_3.AddCell(tablaEquipamientoEncabezado_3);

                var tablaEquipamientoEncabezado_5 = new PdfPCell(new Phrase("Equipamiento de Administrativo", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.Yellow, Colspan = 5 };
                tablaCuerpo_5.AddCell(tablaEquipamientoEncabezado_5);

                var tablaEquipamientoEncabezado_6 = new PdfPCell(new Phrase("Equipamiento de CSTP", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.Yellow, Colspan = 5 };
                tablaCuerpo_6.AddCell(tablaEquipamientoEncabezado_6);

                var tablaEquipamientoEncabezado_7 = new PdfPCell(new Phrase("Equipamiento de Telematica", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.Yellow, Colspan = 5 };
                tablaCuerpo_7.AddCell(tablaEquipamientoEncabezado_7);

                int numeroDeFilas = tablaCarril.Rows.Count;

                bool EquipamientoPlaza = true;
                bool contenido_1 = false;
                bool contenido_2 = false;
                bool contenido_3 = false;
                bool contenido_5 = false;
                bool contenido_6 = false;
                bool contenido_7 = false;

                for (int i = 0; i < numeroDeFilas; i++)
                {
                    int Componente = Convert.ToInt32(tablaCarril.Rows[i]["Componente"]);

                    if (tablaCarril.Rows[i]["Ubicacion"].Equals(1) && (Componente % 100 == 0))
                    {
                        var Descripcion = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Descripcion"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var MarcaModelo = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Marca/Modelo"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Serie = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Serie"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Inventario = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Inventario"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Observaciones = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Observciones"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

                        tablaCuerpo_1.AddCell(Descripcion);
                        tablaCuerpo_1.AddCell(MarcaModelo);
                        tablaCuerpo_1.AddCell(Serie);
                        tablaCuerpo_1.AddCell(Inventario);
                        tablaCuerpo_1.AddCell(Observaciones);

                        contenido_1 = true;
                    }

                    else if (tablaCarril.Rows[i]["Ubicacion"].Equals(2) && (Componente % 100 == 0))
                    {
                        var Descripcion = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Descripcion"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var MarcaModelo = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Marca/Modelo"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Serie = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Serie"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Inventario = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Inventario"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Observaciones = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Observciones"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

                        tablaCuerpo_2.AddCell(Descripcion);
                        tablaCuerpo_2.AddCell(MarcaModelo);
                        tablaCuerpo_2.AddCell(Serie);
                        tablaCuerpo_2.AddCell(Inventario);
                        tablaCuerpo_2.AddCell(Observaciones);

                        contenido_2 = true;
                    }
                    else if (tablaCarril.Rows[i]["Ubicacion"].Equals(3) && (Componente % 100 == 0))
                    {
                        var Descripcion = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Descripcion"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var MarcaModelo = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Marca/Modelo"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Serie = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Serie"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Inventario = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Inventario"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Observaciones = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Observciones"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

                        tablaCuerpo_3.AddCell(Descripcion);
                        tablaCuerpo_3.AddCell(MarcaModelo);
                        tablaCuerpo_3.AddCell(Serie);
                        tablaCuerpo_3.AddCell(Inventario);
                        tablaCuerpo_3.AddCell(Observaciones);

                        contenido_3 = true;
                    }
                    else if (tablaCarril.Rows[i]["Ubicacion"].Equals(5) && (Componente % 100 == 0))
                    {
                        var Descripcion = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Descripcion"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var MarcaModelo = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Marca/Modelo"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Serie = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Serie"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Inventario = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Inventario"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Observaciones = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Observciones"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

                        tablaCuerpo_5.AddCell(Descripcion);
                        tablaCuerpo_5.AddCell(MarcaModelo);
                        tablaCuerpo_5.AddCell(Serie);
                        tablaCuerpo_5.AddCell(Inventario);
                        tablaCuerpo_5.AddCell(Observaciones);

                        contenido_5 = true;
                    }
                    else if (tablaCarril.Rows[i]["Ubicacion"].Equals(6) && (Componente % 100 == 0))
                    {
                        var Descripcion = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Descripcion"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var MarcaModelo = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Marca/Modelo"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Serie = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Serie"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Inventario = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Inventario"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Observaciones = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Observciones"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

                        tablaCuerpo_6.AddCell(Descripcion);
                        tablaCuerpo_6.AddCell(MarcaModelo);
                        tablaCuerpo_6.AddCell(Serie);
                        tablaCuerpo_6.AddCell(Inventario);
                        tablaCuerpo_6.AddCell(Observaciones);

                        contenido_6 = true;
                    }
                    else if (tablaCarril.Rows[i]["Ubicacion"].Equals(7) && (Componente % 100 == 0))
                    {
                        var Descripcion = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Descripcion"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var MarcaModelo = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Marca/Modelo"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Serie = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Serie"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Inventario = new PdfPCell(new Phrase(tablaCarril.Rows[i]["No. de Inventario"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                        var Observaciones = new PdfPCell(new Phrase(tablaCarril.Rows[i]["Observciones"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

                        tablaCuerpo_7.AddCell(Descripcion);
                        tablaCuerpo_7.AddCell(MarcaModelo);
                        tablaCuerpo_7.AddCell(Serie);
                        tablaCuerpo_7.AddCell(Inventario);
                        tablaCuerpo_7.AddCell(Observaciones);

                        contenido_7 = true;
                    }
                }

                PageEventHelperVerticalCAPUFE._carril = carril["Lane"].ToString();

                if (!Equals(carril["Lane"], "Plaza")/* && EquipamientoPlaza == false*/)
                { 
                    doc.NewPage();
                    EquipamientoPlaza = true;
                }
                else if (Equals(carril["Lane"], "Plaza") && contenido_5 == false && contenido_7 == false)
                    EquipamientoPlaza = false;

                if (contenido_5)
                {
                    doc.Add(tablaCuerpo_5);
                }
                else if(Equals(carril["Lane"], "Plaza"))
                {
                    var tablaEquipamientoEncabezadoVacio = new PdfPCell(new Phrase("Equipamiento Administrativo vacio", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 5 };
                    tablaCuerpo_5.AddCell(tablaEquipamientoEncabezadoVacio);
                    doc.Add(tablaCuerpo_5);
                }
                tablaCuerpo_5.DeleteBodyRows();

                if (contenido_7)
                {
                    doc.Add(tablaCuerpo_7);
                }
                else if (Equals(carril["Lane"], "Plaza"))
                {
                    var tablaEquipamientoEncabezadoVacio = new PdfPCell(new Phrase("Equipamiento de Telematica vacio", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 5 };
                    tablaCuerpo_7.AddCell(tablaEquipamientoEncabezadoVacio);
                    doc.Add(tablaCuerpo_7);
                }
                tablaCuerpo_7.DeleteBodyRows();

                if (contenido_1 && !Equals(carril["Lane"], "Plaza"))
                {
                    doc.Add(tablaCuerpo_1);
                }
                else if (!Equals(carril["Lane"], "Plaza"))
                {
                    var tablaEquipamientoEncabezadoVacio = new PdfPCell(new Phrase("Equipamiento de Carril vacio", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 5 };
                    tablaCuerpo_1.AddCell(tablaEquipamientoEncabezadoVacio);
                    doc.Add(tablaCuerpo_1);
                }
                tablaCuerpo_1.DeleteBodyRows();

                if (contenido_2 && !Equals(carril["Lane"], "Plaza"))
                {
                    doc.Add(tablaCuerpo_2);
                }
                else if (!Equals(carril["Lane"], "Plaza"))
                {
                    var tablaEquipamientoEncabezadoVacio = new PdfPCell(new Phrase("Equipamiento de Cabina vacio", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 5 };
                    tablaCuerpo_2.AddCell(tablaEquipamientoEncabezadoVacio);
                    doc.Add(tablaCuerpo_2);
                }
                tablaCuerpo_2.DeleteBodyRows();

                if (contenido_3 && !Equals(carril["Lane"], "Plaza"))
                {
                    doc.Add(tablaCuerpo_3);
                }
                else if (!Equals(carril["Lane"], "Plaza"))
                {
                    var tablaEquipamientoEncabezadoVacio = new PdfPCell(new Phrase("Equipamiento de Piso vacio", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 5 };
                    tablaCuerpo_3.AddCell(tablaEquipamientoEncabezadoVacio);
                    doc.Add(tablaCuerpo_3);
                }
                tablaCuerpo_3.DeleteBodyRows();

                if (contenido_6 )
                {
                    doc.Add(tablaCuerpo_6);
                }
                else
                {
                    var tablaEquipamientoEncabezadoVacio = new PdfPCell(new Phrase("Equipamiento de CSTP vacio", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 5 };
                    tablaCuerpo_6.AddCell(tablaEquipamientoEncabezadoVacio);
                    doc.Add(tablaCuerpo_6);
                }
                tablaCuerpo_6.DeleteBodyRows();

                tablaCarril.Clear();                   
                
            }

            return null;
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
        #endregion
    }
}
