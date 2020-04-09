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

using System.Reflection;
using System.Globalization;

namespace ApiDTC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {

        private readonly PdfConsultasDb _db;
        public PDFController(PdfConsultasDb db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        // GET: api/PDF
        [HttpGet("{refNum}")]
        public IActionResult GetPDF(string refNum)
        {
            try
            {

                Document doc = new Document();
                
                var dataSet = _db.GetStorePDF(refNum);

                DataTable TableHeader = dataSet.Tables[0];
                DataTable TableDTCData = dataSet.Tables[1];
                DataTable TableEquipoMalo = dataSet.Tables[2];
                DataTable TableEquipoPropuesto = dataSet.Tables[3];

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
                iTextSharp.text.Font letritasMiniMini = new iTextSharp.text.Font(fuenteLetrita, 1f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                BaseFont fuenteMini = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                iTextSharp.text.Font letritasMini = new iTextSharp.text.Font(fuenteMini, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);

                if(System.IO.File.Exists($@"{System.Environment.CurrentDirectory}\ReporteDTC-{refNum}.pdf"))
                    System.IO.File.Delete($@"{System.Environment.CurrentDirectory}\ReporteDTC-{refNum}.pdf");
                
                FileStream file = new FileStream(System.Environment.CurrentDirectory + "\\ReporteDTC-" + refNum + ".pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                PdfWriter write = PdfWriter.GetInstance(doc, file);
                doc.AddAuthor("Prosis");
                doc.AddTitle("Reporte Correctivo");
                doc.Open();

                //Logo Prosis
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(System.Environment.CurrentDirectory + "\\prosis-logo.jpg");
                //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("../prosis-logo.jpg");
                logo.ScalePercent(10f);


                //Encabezado
                var tablaEncabezado = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100f };
                var col1 = new PdfPCell(logo) { Border = 0 };
                var col2 = new PdfPCell(new Phrase("DICTAMEN TÉCNICO Y COTIZACION", letraoNegritaMediana)) { BorderWidthTop = 1, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 10, PaddingRight = 20, PaddingLeft = 20 };
                //Creamos Chunk Para dosTipo deLetra en un Parrafo
                var referencia = new Chunk("Referencia:    ", letraNormalMediana);
                var numreferencia = new Chunk(refNum, letraoNegritaMediana);
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
                colAnidada.Phrase = new Phrase("Ultimo Mantenimiento", letraoNegritaChica);
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




                foreach (DataRow item in TableEquipoMalo.Rows)
                {

                    
                    var colPartidaList = new PdfPCell(new Phrase(item["Partida"].ToString(), letritasMini)) { BorderWidth = 1 };

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
                }


                doc.Add(tablaEquipoDanado);



                //doc.Add(new Phrase("EQUIPO PROPUESTO\t\t\t\t\t\t\tDiagnóstico:", letraoNegritaMediana));
                //Tabla Para Letras :(

                var tablaTituloPropuesto = new PdfPTable(new float[] { 50f, 38f, 50f }) { WidthPercentage = 100f };

                var colTitulo1EquipoPropuesto = new PdfPCell(new Phrase("EQUIPO PROPUESTO:", letraoNegritaMediana)) { HorizontalAlignment = Element.ALIGN_LEFT, PaddingLeft = 0, PaddingBottom = 5, PaddingTop = 5, PaddingRight = 0 };
                colTitulo1EquipoPropuesto.Border = 0;

                var colEmpyTitulo = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT, PaddingLeft = 0, PaddingBottom = 5, PaddingTop = 5, PaddingRight = 0 };
                colEmpyTitulo.Border = 0;


                var colTitulo2EquipoPropuesto = new PdfPCell(new Phrase("DIAGNÓSTICO:", letraoNegritaMediana)) { HorizontalAlignment = Element.ALIGN_CENTER, PaddingLeft = 0, PaddingBottom = 5, PaddingTop = 5, PaddingRight = 0 };
                colTitulo2EquipoPropuesto.Border = 0;

                tablaTituloPropuesto.AddCell(colTitulo1EquipoPropuesto);
                tablaTituloPropuesto.AddCell(colEmpyTitulo);
                tablaTituloPropuesto.AddCell(colTitulo2EquipoPropuesto);
                doc.Add(tablaTituloPropuesto);

                //TABLA DE EQUIPO PROPUESTO
                var tablaEquipoPropuesto = new PdfPTable(new float[] { 15f, 15f, 20f, 50f, 20f, 20f, 20f, 20f, 25f, 25f, 20f, 75f }) { WidthPercentage = 100f };
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
                var colPrecioTotalDolarPro = new PdfPCell(new Phrase("Precio Total\nDolares", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                var colVaciaPro = new PdfPCell(new Phrase(" ")) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                colVaciaPro.Border = 0;


                var colDignostico = new PdfPCell();
                colDignostico.Phrase = new Phrase(TableDTCData.Rows[0]["Diagnosis"].ToString(), letraoNegritaMediana);
                colDignostico.BorderWidthTop = 1;
                colDignostico.BorderWidthLeft = 1;
                colDignostico.BorderWidthRight = 1;
                colDignostico.BorderWidthBottom = 0;


                tablaEquipoPropuesto.AddCell(colPartidaPro);
                tablaEquipoPropuesto.AddCell(colUnidadPro);
                tablaEquipoPropuesto.AddCell(colCantidadPro);
                tablaEquipoPropuesto.AddCell(colComponentePro);
                tablaEquipoPropuesto.AddCell(colMarcaPro);
                tablaEquipoPropuesto.AddCell(colModeloPro);
                tablaEquipoPropuesto.AddCell(colPrecioPro);
                tablaEquipoPropuesto.AddCell(colPrecioDolarPro);
                tablaEquipoPropuesto.AddCell(colPrecioTotalPro);
                tablaEquipoPropuesto.AddCell(colPrecioTotalDolarPro);
                tablaEquipoPropuesto.AddCell(colVaciaPro);
                tablaEquipoPropuesto.AddCell(colDignostico);


                int totalFilas = TableEquipoPropuesto.Rows.Count;
                int filaRecorrida = 1;
                double precioTotalMoneda = 0;

                foreach (DataRow item2 in TableEquipoPropuesto.Rows)
                {

                    var colPartidaProList = new PdfPCell(new Phrase(item2["Partida"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colUnidadProList = new PdfPCell(new Phrase(item2["Unidad"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colCantidadProList = new PdfPCell(new Phrase(item2["Cantidad"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colComponenteProList = new PdfPCell(new Phrase(item2["Componente"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colMarcaProList = new PdfPCell(new Phrase(item2["Marca"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colModeloProList = new PdfPCell(new Phrase(item2["Modelo"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colPrecioProList = new PdfPCell(new Phrase("$" + item2["PrecioUnitario"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colPrecioDolarProList = new PdfPCell(new Phrase("$" + item2["PrecoDollarUnitario"].ToString(), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colPrecioTotalProList = new PdfPCell(new Phrase(Convert.ToDouble(item2["PrecioTotal"]).ToString("C", CultureInfo.CurrentCulture), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colPrecioTotalDolarProList = new PdfPCell(new Phrase("", letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                    var colVaciaProList = new PdfPCell(new Phrase(" ")) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };

                    colVaciaProList.Border = 0;


                    var colDignosticoList = new PdfPCell();
                    colDignosticoList.Phrase = new Phrase("");
                    colDignosticoList.BorderWidthLeft = 1;
                    colDignosticoList.BorderWidthRight = 1;
                    colDignosticoList.BorderWidthTop = 0;

                    if (filaRecorrida == totalFilas)
                        colDignosticoList.BorderWidthBottom = 1;
                    else
                        colDignosticoList.BorderWidthBottom = 0;



                    tablaEquipoPropuesto.AddCell(colPartidaProList);
                    tablaEquipoPropuesto.AddCell(colUnidadProList);
                    tablaEquipoPropuesto.AddCell(colCantidadProList);
                    tablaEquipoPropuesto.AddCell(colComponenteProList);
                    tablaEquipoPropuesto.AddCell(colMarcaProList);
                    tablaEquipoPropuesto.AddCell(colModeloProList);
                    tablaEquipoPropuesto.AddCell(colPrecioProList);
                    tablaEquipoPropuesto.AddCell(colPrecioDolarProList);
                    tablaEquipoPropuesto.AddCell(colPrecioTotalProList);
                    tablaEquipoPropuesto.AddCell(colPrecioTotalDolarProList);
                    tablaEquipoPropuesto.AddCell(colVaciaProList);
                    tablaEquipoPropuesto.AddCell(colDignosticoList);

                    precioTotalMoneda += Convert.ToDouble(item2["PrecioTotal"]);

                    filaRecorrida++;

                }



                doc.Add(tablaEquipoPropuesto);

                //Table del Total con Letra
                var tablaTotal = new PdfPTable(new float[] { 75.5f, 10.5f, 10.5f }) { WidthPercentage = 70.8f };
                tablaTotal.HorizontalAlignment = Element.ALIGN_LEFT;


                var colTotalLetra = new PdfPCell(new Phrase(_db.ConverToMoneda(precioTotalMoneda.ToString()), letraoNegritaMediana)) { HorizontalAlignment = Element.ALIGN_LEFT };
                colTotalLetra.Border = 0;
                var colTotalNumero = new PdfPCell(new Phrase(precioTotalMoneda.ToString("C", CultureInfo.CurrentCulture), letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };
                var colTotalDolarNumero = new PdfPCell(new Phrase("", letritasMini)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidth = 1 };

                tablaTotal.AddCell(colTotalLetra);
                tablaTotal.AddCell(colTotalNumero);
                tablaTotal.AddCell(colTotalDolarNumero);

                
                var colRelleno1 = new PdfPCell(new Phrase("TOTAL:", letraoNegritaChica)) { HorizontalAlignment = Element.ALIGN_LEFT };
                colRelleno1.BorderWidthTop = 0;
                colRelleno1.BorderWidthLeft = 0;
                colRelleno1.BorderWidthRight = 0;
                colRelleno1.BorderWidthBottom = 1;

                var colRelleno2 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
                var colRelleno3 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };

                tablaTotal.AddCell(colRelleno1);
                tablaTotal.AddCell(colRelleno2);
                tablaTotal.AddCell(colRelleno3);

                doc.Add(tablaTotal);
                doc.Add(new Phrase(".", letritasMiniMini));

                //Tabla Observaciones + Caja Estatica
                var tablaEstatica = new PdfPTable(new float[] { 40f, 7f, 34f, 7f, 26f }) { WidthPercentage = 100f };

                var colDatosEstaticos = new PdfPCell(new Phrase("Tiempo de entrega:\nVigencia de Cotizacion: 15 dias calendario, a partir de la fecha del presente\nForma de Pago: 100% al termino de los trabajos\nPrecios en M.N No incluye IVA, el cual se cargara al momento de facturarse\nEn caso de una variacion de la paridad Peso/Dolar mayor al 5% se revisaran los precios\nPrecios en USCY: Noincluyen IVA, el cual se cargara al momneto de facturarse\n\n", letraNormalChica)) { HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 1, BorderWidthRight = 1, BorderWidthTop = 1, BorderWidthBottom = 1, Padding = 2 };

                var colEmpy1 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
                colEmpy1.Border = 0;

                var colDatosObservaciones = new PdfPCell(new Phrase("Observaciones\n\n" + TableDTCData.Rows[0]["Observation"].ToString(), letraoNegritaMediana)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 1, BorderWidthRight = 1, BorderWidthTop = 1, BorderWidthBottom = 1 };

                var colEmpy2 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
                colEmpy2.Border = 0;

                var colEmpy3 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT, BorderWidthLeft = 1, BorderWidthRight = 1, BorderWidthTop = 1, BorderWidthBottom = 1 };


                tablaEstatica.AddCell(colDatosEstaticos);
                tablaEstatica.AddCell(colEmpy1);
                tablaEstatica.AddCell(colDatosObservaciones);
                tablaEstatica.AddCell(colEmpy2);
                tablaEstatica.AddCell(colEmpy3);


                doc.Add(tablaEstatica);


                doc.Add(new Phrase(".", letritasMiniMini));

                //Tabla Final
                var tablaFinal = new PdfPTable(new float[] { 40f, 7f, 34f, 7f, 26f }) { WidthPercentage = 100f };

                var colAutorizacion = new PdfPCell(new Phrase("AUTORIZACION TECNICA Y COMERCIAL\n\n\n\n\n\nAutorizacion Comercial Director de Comercializacion\nC.P Hermilia Guzman", letraNormalChica)) { HorizontalAlignment = Element.ALIGN_CENTER, BorderWidthLeft = 1, BorderWidthRight = 1, BorderWidthTop = 1, BorderWidthBottom = 1, Padding = 2 };

                var colEmpy4 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
                colEmpy4.Border = 0;

                var colEmpy5 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_CENTER };
                colEmpy5.Border = 0;

                var colEmpy6 = new PdfPCell(new Phrase("")) { HorizontalAlignment = Element.ALIGN_LEFT };
                colEmpy6.Border = 0;

                var colAdministrador = new PdfPCell(new Phrase("Jose Juan Iturbe Espin\nAdministrador Plaza de Cobro\nc102adm@capufe.gob.mx", letraNormalMediana)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Border = 0 };
                colAdministrador.Border = 0;


                tablaFinal.AddCell(colAutorizacion);
                tablaFinal.AddCell(colEmpy4);
                tablaFinal.AddCell(colEmpy5);
                tablaFinal.AddCell(colEmpy6);
                tablaFinal.AddCell(colAdministrador);

                doc.Add(tablaFinal);

                doc.Add(new Phrase("\n"));
                doc.Add(new Phrase("\n"));
                doc.Add(new Phrase("PROYECTOS Y SISTEMAS INFORMATICOS, S.A DE C.V AV.DOCTOR JOSE MARIA VERTIZ No.1238 INT.1 LETRAN VALLE C.P 03650 BENITO JUAREZ D.F TEL. 44442306", letraNormalMediana));

                write.Close();
                doc.Close();
                file.Close();


                var pdf = new FileStream("ReporteDTC-" + refNum + ".pdf", FileMode.Open, FileAccess.Read);
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {

                string path = @"c:\temporal\CazaErrores.txt";

                if (System.IO.File.Exists(@"C:\temporal\Log.txt"))
                {
                    string[] lineas = System.IO.File.ReadAllLines(@"C:\temporal\Log.txt");
                    lineas[0] = $"PDF. {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}. {Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(" ") + 1))}. {ex.Message}.\n{lineas[0]}";
                    System.IO.File.Delete(@"C:\temporal\Log.txt");
                    System.IO.File.WriteAllLines(@"C:\temporal\Log.txt", lineas);
                }
                else System.IO.File.WriteAllText(@"C:\temporal\Log.txt", $"PDF. {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}. {Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(" ") + 1))}. {ex.Message}.");
                return null;
            }
        }

 
    }


}
