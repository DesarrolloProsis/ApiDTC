namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    public class FichaTecnicaDiagnostico
    {
        [StringLength(30)]
        public string ReferenceNumber { get; set; }

        [StringLength(4)]
        public string SquareId { get; set; }

        [StringLength(10)]
        public string CapufeLaneNum { get; set; }

        [StringLength(3)]
        public string IdGare { get; set; }

        public DateTime DiagnosisDate { get; set; }

        [StringLength(30)]
        public string Start { get; set; }

        [StringLength(30)]
        public string End { get; set; }

        [StringLength(30)]
        public string FailureNumber { get; set; }

        public int UserId { get; set; }

        [StringLength(300)]
        public string FailureDescription { get; set; }

        [StringLength(300)]
        public string FailureDiagnosis { get; set; }

        [StringLength(300)]
        public string CauseFailure { get; set; }

        public int AdminSquareId { get; set; }
    }
}