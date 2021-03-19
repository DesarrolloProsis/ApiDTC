namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DiagnosticoDeFalla
    {
        [Required]
        [StringLength(30)]
        public string ReferenceNumber { get; set; }

        [Required]
        [StringLength(4)]
        public string SquareId { get; set; }

        [Required]
        public DateTime DiagnosisDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Start { get; set; }

        [Required]
        [StringLength(30)]
        public string End { get; set; }

        [Required]
        [StringLength(20)]
        public string SinisterNumber { get; set; }

        [Required]
        [StringLength(30)]
        public string FailureNumber { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(300)]
        public string FailureDescription { get; set; }

        [Required]
        [StringLength(300)]
        public string FailureDiagnosis { get; set; }

        [Required]
        [StringLength(300)]
        public string CauseFailure { get; set; }

        [Required]
        public int AdminSquareId { get; set; }

        [Required]
        public bool UpdateFlag { get; set; }
    }
}