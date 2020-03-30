using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using ApiDTC.Data;
using System.Data;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {

        private readonly PDFConsultasDB _db;
        public PDFController(PDFConsultasDB db)
        {

            this._db = db ?? throw new ArgumentNullException(nameof(db));

        }
        // GET: api/PDF
        [HttpGet("{refNum}")]
        public IActionResult GetPDF(string refNum)
        {
            
            
            Document doc = new Document();

            var dataSet = _db.GetStorePDF(refNum);

            DataTable TableHeader = dataSet.Tables[0];
            DataTable TableDTCData = dataSet.Tables[1];
            DataTable TableEquipoMalo = dataSet.Tables[2];



            //Tamaña del documente
            doc.SetPageSize(new Rectangle(793.701f, 609.4488f));
            doc.SetMargins(70.8661f, 42.5197f, 28.3465f, 28.3465f);
            //Tipo de Letras 
            BaseFont fuenteTitulos = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
  
            BaseFont NegritaGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letraoNegritaGrande = new iTextSharp.text.Font(NegritaGrande, 15f, iTextSharp.text.Font.BOLD, BaseColor.Black);

            BaseFont NegritaMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letraoNegritaMediana = new iTextSharp.text.Font(NegritaMediana, 7f, iTextSharp.text.Font.BOLD, BaseColor.Black);

            BaseFont NegritaChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letraoNegritaChica = new iTextSharp.text.Font(NegritaChica, 5f, iTextSharp.text.Font.BOLD, BaseColor.Black);

            BaseFont NormalGrande = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letraNormalGrande = new iTextSharp.text.Font(NormalGrande, 15f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

            BaseFont NormalMediana = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letraNormalMediana = new iTextSharp.text.Font(NormalMediana, 7f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

            BaseFont NormalChica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letraNormalChica = new iTextSharp.text.Font(NormalChica, 5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);


            BaseFont NormalChicaSubAzul = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letraSubAzulChica = new iTextSharp.text.Font(NormalChicaSubAzul, 5f, iTextSharp.text.Font.UNDERLINE, BaseColor.Blue);





            BaseFont fuenteLetrita = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letritas = new iTextSharp.text.Font(fuenteLetrita, 10f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
            BaseFont fuenteMini = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);




            FileStream file = new FileStream("ReporteDTC.pdf-"+refNum, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            PdfWriter write = PdfWriter.GetInstance(doc, file);
            doc.AddAuthor("Prosis");
            doc.AddTitle("Reporte Correctivo");
            doc.Open();

            //Logo Prosis
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("C:/Users/Desarrollo3/Desktop/ApiDTC-NetCore2.1/ApiDTC/prosis-logo.jpg");
            logo.ScalePercent(10f);


          //Encabezado
            var tablaEncabezado = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100f };
            var col1 = new PdfPCell(logo) { Border = 0 };
            var col2 = new PdfPCell(new Phrase("DICTAMEN TÉCNICO Y COTIZACION", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 10, PaddingRight = 20, PaddingLeft = 20 };
            //Creamos Chunk Para dosTipo deLetra en un Parrafo
            var referencia = new Chunk("Referencia:    ", letraNormalMediana);
            var numreferencia = new Chunk("PMO-20068", letraoNegritaMediana);
            var refCompleta = new Phrase(referencia);
            refCompleta.Add(numreferencia);
            var col3 = new PdfPCell(refCompleta) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE };

            tablaEncabezado.AddCell(col1);
            tablaEncabezado.AddCell(col2);
            tablaEncabezado.AddCell(col3);
            doc.Add(tablaEncabezado);


            doc.Add(new Phrase(" "));
            doc.Add(new Phrase(" "));
            doc.Add(new Phrase(" "));


            //Sub Encabezado
            var tablaSiniestro = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100f };
            //Agregamos Chunk Para 2 letras
            var contratoOferta = new Chunk("   Contrato/Oferta:                           ", letraNormalChica);
            var numContratoOferta = new Chunk(Convert.ToString(Convert.ToString(TableHeader.Rows[0]["Agrement"])), letraoNegritaChica);
            var ContratoOfertaCompleto = new Phrase(contratoOferta);
            ContratoOfertaCompleto.Add(numContratoOferta);
            var col4 = new PdfPCell(new Phrase(ContratoOfertaCompleto)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
            //-----------------------------------------
            var col5 = new PdfPCell(new Phrase("EN CASO DE SINIESTRO", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE };
            //-----------------------------------------
            //Agregamos Chunk 2 letras
            var tipoDictamen = new Chunk("Tipo de Dictamen        ", letraNormalChica);
            var dictamen = new Chunk("CORRECTIVO", letraoNegritaChica);
            var tipoDictamenCompleto = new Phrase(tipoDictamen);
            tipoDictamenCompleto.Add(dictamen);
            var col6 = new PdfPCell(new Phrase(tipoDictamenCompleto)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE };
            //Agregamos Chunk 2 letras
            var atencion = new Chunk("Atención:   ", letraNormalChica);
            var atencionNombre = new Chunk(Convert.ToString(TableHeader.Rows[0]["ManagerName"]), letraNormalChica);
            var atencionCompleto = new Phrase(atencion);
            atencionCompleto.Add(atencionNombre);
            var col7 = new PdfPCell(new Phrase(atencionCompleto)) { Border = 0 };
            //-----------------------------------------
            var col8 = new PdfPCell(new Phrase(" ")) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
            //Agregamos Chunk 2 Letras
            var descripcion = new Chunk("Descripcion:          ", letraNormalChica);
            var descripcionNombre = new Chunk(Convert.ToString(TableDTCData.Rows[0]["TipoDescripicon"]), letraoNegritaChica);
            var descripcionCompleta = new Phrase(descripcion);
            descripcionCompleta.Add(descripcionNombre);
            var col9 = new PdfPCell(new Phrase(descripcionCompleta)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE };
            //-----------------------------------------
            tablaSiniestro.AddCell(col4);
            tablaSiniestro.AddCell(col5);
            tablaSiniestro.AddCell(col6);
            tablaSiniestro.AddCell(col7);
            tablaSiniestro.AddCell(col8);
            tablaSiniestro.AddCell(col9);
            doc.Add(tablaSiniestro);

            //Continuacion
            var tablaSiniestroMore = new PdfPTable(new float[] { 35f, 35f, 35f }) { WidthPercentage = 100f };

            var col10 = new PdfPCell(new Phrase("Cargo:  " + "   " + Convert.ToString(TableHeader.Rows[0]["Position"]), letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, Padding = 1f, VerticalAlignment = Element.ALIGN_CENTER };
            //Agregamos Chunk 4 Letras
            var siniestro = new Chunk("No.Siniestro    ", letraNormalChica);
            var numSiniestro = new Chunk(Convert.ToString(TableDTCData.Rows[0]["SinisterNumber"]), letraoNegritaChica);
            var reporte = new Chunk("Reporte          ", letraNormalChica);
            var numReporte = new Chunk(Convert.ToString(TableDTCData.Rows[0]["ReportNumber"]), letraoNegritaChica);
            var numSiniestroCompleto = new Phrase(siniestro);
            numSiniestroCompleto.Add(numSiniestro);
            numSiniestroCompleto.Add("               ");
            numSiniestroCompleto.Add(reporte);
            numSiniestroCompleto.Add(numReporte);
            var col11 = new PdfPCell(new Phrase(numSiniestroCompleto)) { Border = 0 };
            //Agregamo Chunk 2 Letras
            var lugarFecha = new Chunk("Lugar de Fecha de Envio:   ", letraNormalChica);
            var lugarFechaText = new Chunk("CDMX a " + Convert.ToDateTime(TableDTCData.Rows[0]["ElaborationDate"]).ToString("dd/MM/yyyy"), letraoNegritaChica);
            var lugarFechaComplete = new Phrase(lugarFecha);
            lugarFechaComplete.Add(lugarFechaText);
            var col12 = new PdfPCell(new Phrase(lugarFechaComplete)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
            //Agregar Chunk 2 Letras
            var correo = new Chunk("Correo:     ", letraNormalChica);
            var CorreoText = new Chunk(Convert.ToString(TableHeader.Rows[0]["Mail"]), letraSubAzulChica);
            var correoCompleto = new Phrase(correo);
            correoCompleto.Add(CorreoText);
            var col13 = new PdfPCell(new Phrase(correoCompleto)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, Padding = 1f };
            var col14 = new PdfPCell(new Phrase("Fecha Siniestro:" + ' ' + Convert.ToDateTime(TableDTCData.Rows[0]["SinisterDate"]).ToString("dd/MM/yyyy"), letraNormalChica)) { Border = 0 };
            //Agregamos Chunk 2 Letras 
            var tecnicoResponsable = new Chunk("Tecnico Responsable:  ", letraNormalChica);
            var tecnicoResponsableText = new Chunk(Convert.ToString(TableDTCData.Rows[0]["TecnicoResponsable"]), letraNormalChica);
            var tecnicoResponsableCompleto = new Phrase(tecnicoResponsable);
            tecnicoResponsableCompleto.Add(tecnicoResponsableText);
            var col15 = new PdfPCell(new Phrase(tecnicoResponsableCompleto)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };


            var col16 = new PdfPCell(new Phrase("Plaza de Cobro:" + "      " + Convert.ToString(TableHeader.Rows[0]["Plaza"]), letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, Padding = 1f };
            var col17 = new PdfPCell(new Phrase("Folio(s) Fallas(s):" + ' ', letraNormalChica)) { Border = 0, };
            var col18 = new PdfPCell(new Phrase("Coordinacion Regional:" + ' ' + Convert.ToString(TableHeader.Rows[0]["RegionalCoordination"]), letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };

            var col19 = new PdfPCell(new Phrase(" ", letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, Padding = 1f };
            var col20 = new PdfPCell(new Phrase("Fecha de Falla:" + ' ' + Convert.ToDateTime(TableDTCData.Rows[0]["FailureDate"]).ToString("dd/MM/yyyy"), letraNormalChica)) { Border = 0 };
            var col21 = new PdfPCell(new Phrase("Centro de Servicio:" + "                     " + "\nFecha de Elaboracion:" + ' ' + Convert.ToDateTime(TableDTCData.Rows[0]["ElaborationDate"]).ToString("dd/MM/yyyy"), letraNormalChica)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };


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

            doc.Add(tablaSiniestroMore);



            //doc.Add(new Phrase(" "));
            doc.Add(new Phrase(" "));
            doc.Add(new Phrase("EQUIPO DAÑADO", letraoNegritaMediana));
            //TablaEquipo Dañado
            var tablaEquipoDanado = new PdfPTable(new float[] { 10f, 10f, 10f, 50f, 20f, 20f, 20f, 20f, 25f, 35f, 35f }) { WidthPercentage = 100f };

            var colPartida = new PdfPCell(new Phrase("Partida", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colUnidad = new PdfPCell(new Phrase("Unidad", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colCantidad = new PdfPCell(new Phrase("Cantidad", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colComponente = new PdfPCell(new Phrase("Componente", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colMarca = new PdfPCell(new Phrase("Marca", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colModelo = new PdfPCell(new Phrase("Modelo", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colNumSerie = new PdfPCell(new Phrase("Num. Serie", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colUbicacion = new PdfPCell(new Phrase("Ubicacion\n(Carril)", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colFechaInstalacion = new PdfPCell(new Phrase("Fecha Instalacion\n(dd-mm-aa)", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            //var colUltimoMantenimiento = new PdfPCell(new Phrase("Ultimo Mantenimiento", letritasMini)) {  };
            //var colFinVidaUtil = new PdfPCell(new Phrase("Fin de Vida Util", letritasMini)) { };

            //Tabla Anidada de UltimoMantenimiento 'Remplazo de colUltimoMantenimiento'
            var tablaEquipoDanadoAnidada = new PdfPTable(new float[] { 35f, 35f }) { WidthPercentage = 100f };

            PdfPCell colAnidada = new PdfPCell();
            PdfPCell colAnidada2 = new PdfPCell();
            colAnidada.Colspan = 2;
            colAnidada.Phrase = new Phrase("Anidado", letraoNegritaChica);
            colAnidada.HorizontalAlignment = Element.ALIGN_CENTER;
            colAnidada.Border = 0;            
            colAnidada.BorderWidthBottom = 1;
            tablaEquipoDanadoAnidada.AddCell(colAnidada);

            colAnidada2.Phrase = new Phrase("Fecha", letraoNegritaChica);
            colAnidada2.Border = 1;
            colAnidada2.BorderWidthRight = 1;
            tablaEquipoDanadoAnidada.AddCell(colAnidada2);
            colAnidada2.Phrase = new Phrase("Folio", letraoNegritaChica);
            colAnidada2.Border = 0;
            tablaEquipoDanadoAnidada.AddCell(colAnidada2);            

            //Tabla Anidada de UltimoMantenimiento 'Remplazo de colFinVidaUtil'
            var tablaEquipoDanadoAnidada2 = new PdfPTable(new float[] { 35f, 35f }) { WidthPercentage = 100f };

            PdfPCell colAnidada3 = new PdfPCell();
            PdfPCell colAnidada4 = new PdfPCell();

            colAnidada3.Colspan = 2;
            colAnidada3.Phrase = new Phrase("Fin de Vida Util", letraoNegritaChica);
            colAnidada3.HorizontalAlignment = Element.ALIGN_CENTER;
            colAnidada3.Border = 0;
            colAnidada3.BorderWidthBottom = 1;
            tablaEquipoDanadoAnidada2.AddCell(colAnidada3);

            colAnidada4.Phrase = new Phrase("Real", letraoNegritaChica);
            colAnidada4.Border = 0;
            colAnidada4.BorderWidthRight = 1;
            tablaEquipoDanadoAnidada2.AddCell(colAnidada4);
            colAnidada4.Phrase = new Phrase("Fabricantes", letraoNegritaChica);
            colAnidada4.Border = 0;
            tablaEquipoDanadoAnidada2.AddCell(colAnidada4);




            tablaEquipoDanado.AddCell(colPartida);
            tablaEquipoDanado.AddCell(colUnidad);
            tablaEquipoDanado.AddCell(colCantidad);
            tablaEquipoDanado.AddCell(colComponente);
            tablaEquipoDanado.AddCell(colMarca);
            tablaEquipoDanado.AddCell(colModelo);
            tablaEquipoDanado.AddCell(colNumSerie);
            tablaEquipoDanado.AddCell(colUbicacion);
            tablaEquipoDanado.AddCell(colFechaInstalacion);
            tablaEquipoDanado.AddCell(tablaEquipoDanadoAnidada);
            tablaEquipoDanado.AddCell(tablaEquipoDanadoAnidada2);



            int i = 1;
            foreach (DataRow item in TableEquipoMalo.Rows)
            {


                var colPartidaList = new PdfPCell(new Phrase(Convert.ToString(i), letritasMini)) { BorderWidth = 1 };

                var colUnidadList = new PdfPCell(new Phrase(item["Unidad"].ToString(), letritasMini)) { BorderWidth = 1 };
                var colCantidadList = new PdfPCell(new Phrase(item["Cantidad"].ToString(), letritasMini)) { BorderWidth = 1 };
                var colComponenteList = new PdfPCell(new Phrase(item["Componente"].ToString(), letritasMini)) { BorderWidth = 1 };
                var colMarcaList = new PdfPCell(new Phrase(item["Marca"].ToString(), letritasMini)) { BorderWidth = 1 };
                var colModeloList = new PdfPCell(new Phrase(item["Modelo"].ToString(), letritasMini)) { BorderWidth = 1 };
                var colNumSerieList = new PdfPCell(new Phrase(item["NumeroSerie"].ToString(), letritasMini)) { BorderWidth = 1 };
                var colUbicacionList = new PdfPCell(new Phrase(item["Ubicacion"].ToString(), letritasMini)) { BorderWidth = 1 };
                var colFechaInstalacionList = new PdfPCell(new Phrase(item["FechaInstalacion"].ToString(), letritasMini)) { BorderWidth = 1 };

                //Tabla Anidada de UltimoMantenimiento 'Remplazo de colUltimoMantenimiento'
                var tablaEquipoDanadoAnidadaList = new PdfPTable(new float[] { 35f, 35f }) { WidthPercentage = 100f };

                PdfPCell colAnidada2List = new PdfPCell();

                colAnidada2List.Phrase = new Phrase("31/01/2020", letritasMini);
                colAnidada2List.Border = 0;
                colAnidada2List.BorderWidthRight = 1;
                tablaEquipoDanadoAnidadaList.AddCell(colAnidada2List);
                colAnidada2List.Phrase = new Phrase("S/M", letritasMini);
                colAnidada2List.Border = 0;
                tablaEquipoDanadoAnidadaList.AddCell(colAnidada2List);

                //Tabla Anidada de UltimoMantenimiento 'Remplazo de colFinVidaUtil'
                var tablaEquipoDanadoAnidada2List = new PdfPTable(new float[] { 35f, 35f }) { WidthPercentage = 100f };


                PdfPCell colAnidada4List = new PdfPCell();


                colAnidada4List.Phrase = new Phrase("-------", letritasMini);
                colAnidada4List.Border = 0;
                colAnidada4List.BorderWidthRight = 1;
                tablaEquipoDanadoAnidada2List.AddCell(colAnidada4List);
                colAnidada4List.Phrase = new Phrase("-------", letritasMini);
                colAnidada4List.Border = 0;
                tablaEquipoDanadoAnidada2List.AddCell(colAnidada4List);


                tablaEquipoDanado.AddCell(colPartidaList);
                tablaEquipoDanado.AddCell(colUnidadList);
                tablaEquipoDanado.AddCell(colCantidadList);
                tablaEquipoDanado.AddCell(colComponenteList);
                tablaEquipoDanado.AddCell(colMarcaList);
                tablaEquipoDanado.AddCell(colModeloList);
                tablaEquipoDanado.AddCell(colNumSerieList);
                tablaEquipoDanado.AddCell(colUbicacionList);
                tablaEquipoDanado.AddCell(colFechaInstalacionList);
                tablaEquipoDanado.AddCell(tablaEquipoDanadoAnidadaList);
                tablaEquipoDanado.AddCell(tablaEquipoDanadoAnidada2List);

                i++;

            }


            doc.Add(tablaEquipoDanado);



            doc.Add(new Phrase("EQUIPO PROPUESTO", letraoNegritaMediana));


            //TABLA DE EQUIPO PROPUESTO
            var tablaEquipoPropuesto = new PdfPTable(new float[] { 15f, 15f, 20f, 50f, 20f, 20f, 20f, 20f, 25f, 20f, 75f }) { WidthPercentage = 100f };
            tablaEquipoPropuesto.HorizontalAlignment = Element.ALIGN_LEFT;

            var colPartidaPro = new PdfPCell(new Phrase("Partida", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colUnidadPro = new PdfPCell(new Phrase("Unidad", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colCantidadPro = new PdfPCell(new Phrase("Cantidad", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colComponentePro = new PdfPCell(new Phrase("Componente", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colMarcaPro = new PdfPCell(new Phrase("Marca", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colModeloPro = new PdfPCell(new Phrase("Modelo", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colPrecioPro = new PdfPCell(new Phrase("Precio\nUnitario Pesos M.N", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colPrecioDolarPro = new PdfPCell(new Phrase("Precio\nUnitario Dolares", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colPrecioTotalPro = new PdfPCell(new Phrase("Precio Total\nPesos M.N", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            var colVaciaPro = new PdfPCell(new Phrase(" ")) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
            colVaciaPro.Border = 0;


            var colDignostico = new PdfPCell();
            colDignostico.Phrase = new Phrase("Diagnotico");

            tablaEquipoPropuesto.AddCell(colPartidaPro);
            tablaEquipoPropuesto.AddCell(colUnidadPro);
            tablaEquipoPropuesto.AddCell(colCantidadPro);
            tablaEquipoPropuesto.AddCell(colComponentePro);
            tablaEquipoPropuesto.AddCell(colMarcaPro);
            tablaEquipoPropuesto.AddCell(colModeloPro);
            tablaEquipoPropuesto.AddCell(colPrecioPro);
            tablaEquipoPropuesto.AddCell(colPrecioDolarPro);
            tablaEquipoPropuesto.AddCell(colPrecioTotalPro);
            tablaEquipoPropuesto.AddCell(colVaciaPro);
            tablaEquipoPropuesto.AddCell(colDignostico);

            doc.Add(tablaEquipoPropuesto);
            write.Close();
            doc.Close();
            file.Dispose();

            var pdf = new FileStream("ReporteDTC.pdf-"+refNum, FileMode.Open, FileAccess.Read);
            return File(pdf, "application/pdf");
        }


    }

    
}
