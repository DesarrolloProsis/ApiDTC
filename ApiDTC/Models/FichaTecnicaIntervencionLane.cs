namespace ApiDTC.Models
{
    using System.ComponentModel.DataAnnotations;
    
    public class FichaTecnicaIntervencionLane
    {
        [StringLength(30)]
        public string ReferenceNumber { get; set; }

        [StringLength(10)]
        public string CapuLaneNum { get; set; }

        [StringLength(3)]
        public string IdGare { get; set; }

        public bool AddFlag { get; set; }
    }
}