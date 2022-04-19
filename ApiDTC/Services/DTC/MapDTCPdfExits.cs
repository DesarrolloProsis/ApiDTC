using ApiDTC.Models;
using ApiDTC.Models.DTCGMMEP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Services.DTC
{
    public class MapDTCPdfExits
    {
        string _DISK;
        string _FOLDER;
        public MapDTCPdfExits(string disk, string folder)
        {
            _DISK = disk;
            _FOLDER = folder;
        }

        public List<DTCHeaderWhitPdfValid> ValidarPdf(List<DTCHeaderPaginacion> listDTC)
        {
            List<DTCHeaderWhitPdfValid> dtcViewInfo = new List<DTCHeaderWhitPdfValid>();            

            foreach (var dtcView in listDTC)
            {
               DTCHeaderWhitPdfValid viewInfo = new DTCHeaderWhitPdfValid();                
               viewInfo.DtcView = dtcView;

               
                string path = $@"{_DISK}:\{_FOLDER}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\DTC\{dtcView.ReferenceNumber}\DTC-{dtcView.ReferenceNumber}-Sellado.pdf";
                if (System.IO.File.Exists((path)))
                    viewInfo.PdfDTCExists = true;
                else
                    viewInfo.PdfDTCExists = false;

                //Validacion ReporteFotografico Sellado
                string pathFotograficoSellado = $@"{_DISK}:\{_FOLDER}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\DTC\{dtcView.ReferenceNumber}\DTC-{dtcView.ReferenceNumber}-EquipoDañadoSellado.pdf";
                if (System.IO.File.Exists((pathFotograficoSellado)))
                    viewInfo.PdfFotograficoSellado = true;
                else
                    viewInfo.PdfFotograficoSellado = false;

                string directoy = $@"{_DISK}:\{_FOLDER}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\DTC\{dtcView.ReferenceNumber}\EquipoDañadoImgs";
                List<string> dtcImages = new List<string>();
                if (Directory.Exists(directoy))
                {
                    if (Directory.GetFiles(directoy) != null)
                    {
                        foreach (var item in Directory.GetFiles(directoy))
                            dtcImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                    }
                }
                viewInfo.Paths = dtcImages;

                //IMAGENES DEL DIAGNOSTICO DE FALLA
                string directoyDF = $@"{_DISK}:\{_FOLDER}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\Reportes\{dtcView.TechnicalSheetReference}\DiagnosticoFallaImgs";
                List<string> DiagnosticoFallaImages = new List<string>();
                if (Directory.Exists(directoyDF))
                {
                    if (Directory.GetFiles(directoyDF) != null)
                    {
                        foreach (var item in Directory.GetFiles(directoyDF))
                            DiagnosticoFallaImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                    }
                }
                viewInfo.PathImgDiagnosticoFalla = DiagnosticoFallaImages;       
                
                //IMAGENES DE LA FICHA TECNICA DE ATENCION
                string directoyFA = $@"{_DISK}:\{_FOLDER}\{dtcView.ReferenceNumber.Split('-')[0].ToUpper()}\Reportes\{dtcView.TechnicalSheetReference}\FichaTecnicaAtencionImgs";
                List<string> FichaAtencionImages = new List<string>();
                if (Directory.Exists(directoyFA))
                {
                    if (Directory.GetFiles(directoyFA) != null)
                    {
                        foreach (var item in Directory.GetFiles(directoyFA))
                            FichaAtencionImages.Add(item.Substring(item.LastIndexOf('\\') + 1));
                    }
                }
                viewInfo.PathImgAtencion = FichaAtencionImages;
                dtcViewInfo.Add(viewInfo);
            }
            return dtcViewInfo;
        }
    }
}
