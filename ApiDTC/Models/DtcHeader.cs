namespace ApiDTC.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DtcHeader
    {
        [Required]
        [StringLength(30)]
        public string ReferenceNumber { get; set; }

        [StringLength(30)]
        public string NumSiniestro { get; set; }

        [StringLength(20)]
        public string NumReporte { get; set; }

        [StringLength(60)]
        public string FolioFalla { get; set; }

        [Required]
        public int TipoDescripcion { get; set; }

        [StringLength(300)]
        public string Observaciones { get; set; }

        [StringLength(300)]
        public string Diagnostico { get; set; }
    }
}
