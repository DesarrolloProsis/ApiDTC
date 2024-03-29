namespace ApiDTC.Models
{
    using System;
    using System.Collections.Generic;

    public class DtcViewInfo
    {
        public DtcView DtcView { get; set; }
        public List<string> Paths { get; set; }
        public bool PdfExists { get; set; }
        public bool PdfFotograficoSellado { get; set; }
        public List<string> PathImagesDF { get; set; }
        public List<string> PathImagesFAtencion { get; set; }
    }
}