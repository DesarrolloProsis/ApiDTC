namespace ApiDTC.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DtcHeader
    {
        public string ReferenceNumber { get; set; }

        public string NumSiniestro { get; set; }

        public string NumReporte { get; set; }

        public string FolioFalla { get; set; }

        public int TipoDescripcion { get; set; }

        [StringLength(300)]
        public string Observaciones { get; set; }

        public string Diagnostico { get; set; }
    }
}
