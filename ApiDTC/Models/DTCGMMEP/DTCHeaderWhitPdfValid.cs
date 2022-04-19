using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.DTCGMMEP
{
    public class DTCHeaderWhitPdfValid
    {
        public DTCHeaderPaginacion DtcView { get; set; }
        public bool PdfDTCExists { get; set; }
        public bool PdfFotograficoSellado { get; set; }
        public List<string> Paths { get; set; }
        public List<string> PathImgDiagnosticoFalla { get; set; }
        public List<string> PathImgAtencion { get; set; }
    }
}
