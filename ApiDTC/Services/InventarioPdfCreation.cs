

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

        private readonly DataTable _Piso;

        private readonly DataTable _Carril;

        private readonly DataTable _Cabina;

        private readonly DataTable _CSPT;

        private readonly DataTable _Administracion;

        private readonly DataTable _Telematica;

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

        public InventarioPdfCreation(string clavePlaza,DataTable Informacion, DataTable Administracion, DataTable Telematica, DataTable Carril, DataTable Cabina, DataTable Piso, DataTable CSTP, ApiLogger apiLogger)
        {
            _Informacion = Informacion;
            _Carril = Carril;
            _CSPT = CSTP;
            _Piso = Piso;
            _Cabina = Cabina;
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
            _Administracion = Administracion;
            _Telematica = Telematica;
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
                    writer.PageEvent = new PageEventHelperVerticalCAPUFE(_Informacion);
                    writer.Open();
                    doc.Open();

                    
                    //doc.Add(TablaEncabezado());
                    //doc.Add(CeldasPaginaUno());
                   
                    doc.Add(TablaInformacion(_Administracion, "Administracion"));
                    doc.Add(TablaInformacion(_Telematica, "Telematica"));
                    tableSort(_Carril, doc);
                    //doc.Add(TablaInformacion(_Carril, "Carril"));
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

        //public void CeldasVacias(int numeroCeldas, PdfPTable table)
        //{
        //    for (int i = 0; i < numeroCeldas; i++)
        //        table.AddCell(new PdfPCell() { Border = 0 });
        //}

        //private IElement TablaEncabezado()
        //{
        //    try
        //    {

        //        PdfPTable table = new PdfPTable(new float[] { 16.67f, 16.67f, 16.67f, 16.67f, 16.67f, 16.67f }) { WidthPercentage = 100f };



        //        var colTitulo = new PdfPCell(new Phrase("ANEXO 1.13 FORMATO PARA EL INFORME DE INVENTARIO DE EQUIPOS Y COMPONENTES DE PEAJE AL INICIAR LA VIGENCIA DE LOS SERVICIOS", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, Colspan = 6 };
        //        table.AddCell(colTitulo);
                
        //        CeldasVacias(5, table);
        //        var colFecha = new PdfPCell(new Phrase("Fecha: 01/01/2021", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
        //        table.AddCell(colFecha);

        //        CeldasVacias(1, table);
        //        var colPlaza = new PdfPCell(new Phrase("PLAZA DE COBRO: No. 004", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
        //        var colNombre = new PdfPCell(new Phrase("Nombre: TEPOTZOTLÁN", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
        //        var colCarril = new PdfPCell(new Phrase("Carril: B18 MULTIMODAL", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

        //        table.AddCell(colPlaza);
        //        table.AddCell(colNombre);
        //        table.AddCell(colCarril);
        //        CeldasVacias(2, table);

        //        return table;

        //    }

        
        //    catch (PdfException ex)
        //    {
        //        _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 5);
        //        return null;
        //    }
        //    catch(Exception ex)
        //    {
        //        _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 3);
        //        return null;
        //    }
            
        //}

        //private IElement CeldasPaginaUno()
        //{
        //    try
        //    {
        //        PdfPTable table2 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
        //        var encabezadoDescripcion = new PdfPCell(new Phrase("Descripción", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
        //        //var encabezadoDetalle = new PdfPCell(new Phrase("Detalle", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
        //        var encabezadoMarcaModelo = new PdfPCell(new Phrase("Marca/Modelo", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
        //        //var encabezadoModelo = new PdfPCell(new Phrase("Modelo", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
        //        var encabezadoSerie = new PdfPCell(new Phrase("No. de Serie", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
        //        var encabezadoInventario = new PdfPCell(new Phrase("No. de Inventario", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
        //        //var encabezadoItem = new PdfPCell(new Phrase("ITEM", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };
        //        var encabezadoObservaciones = new PdfPCell(new Phrase("Observaciones", letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_MIDDLE, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.LightGray };


        //        //table2.AddCell(Equipamiento);
        //        table2.AddCell(encabezadoDescripcion);
        //        //table.AddCell(encabezadoDetalle);
        //        table2.AddCell(encabezadoMarcaModelo);
        //        //table.AddCell(encabezadoModelo);
        //        table2.AddCell(encabezadoSerie);
        //        table2.AddCell(encabezadoInventario);
        //        //table.AddCell(encabezadoItem);
        //        table2.AddCell(encabezadoObservaciones);

        //        return table2;
        //    }
        //    catch (PdfException ex)
        //    {
        //        _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 5);
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaEncabezado", 3);
        //        return null;
        //    }

        //}
        private IElement tableSort(DataTable Equipamiento, Document doc)
        {
            PdfPTable tablaCuerpo_1 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpo_2 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            PdfPTable tablaCuerpo_3 = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
            
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

                int numeroDeFilas = tablaCarril.Rows.Count;
                

                for (int i = 0; i < numeroDeFilas ; i++)
                {
                    if (tablaCarril.Rows[i]["Ubicacion"].Equals(1))
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
                    }

                    else if (tablaCarril.Rows[i]["Ubicacion"].Equals(2))
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

                    }
                    else if (tablaCarril.Rows[i]["Ubicacion"].Equals(3))
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
                    }
                    

                }

                doc.Add(tablaCuerpo_1);
                tablaCuerpo_1.DeleteBodyRows();

                doc.Add(tablaCuerpo_2);
                tablaCuerpo_2.DeleteBodyRows();

                doc.Add(tablaCuerpo_3);
                tablaCuerpo_3.DeleteBodyRows();

                doc.NewPage();
                tablaCarril.Clear();
                
            }

            return null;
        }
        private IElement TablaInformacion(DataTable Equipamiento, string Nombre)
        
        {
            try
            {
                PdfPTable tablaCuerpo = new PdfPTable(new float[] { 30f, 20f, 10f, 10f, 30f }) { WidthPercentage = 100f };
                var tablaEquipamiento = new PdfPCell(new Phrase("Equipamiento de " + Nombre, letraoNegritaChica)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3, BackgroundColor = BaseColor.Yellow, Colspan = 5 };
                tablaCuerpo.AddCell(tablaEquipamiento);

                foreach (DataRow item in Equipamiento.Rows)
                {
                    var Descripcion = new PdfPCell(new Phrase(item["Descripcion"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3};
                    var MarcaModelo = new PdfPCell(new Phrase(item["Marca/Modelo"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                    var Serie = new PdfPCell(new Phrase(item["No. de Serie"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                    var Inventario = new PdfPCell(new Phrase(item["No. de Inventario"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };
                    var Observaciones = new PdfPCell(new Phrase(item["Observciones"].ToString(), letritasMini)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_CENTER, Padding = 3 };

                    tablaCuerpo.AddCell(Descripcion);
                    tablaCuerpo.AddCell(MarcaModelo);
                    tablaCuerpo.AddCell(Serie);
                    tablaCuerpo.AddCell(Inventario);
                    tablaCuerpo.AddCell(Observaciones);

                }

                return tablaCuerpo;
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaInformacion", 5);
                return null;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: TablaInformacion", 3);
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
