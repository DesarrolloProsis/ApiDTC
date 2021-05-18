namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DtcData
    {
        #region Properties

        [Required]        
        public string ReferenceNumber { get; set; }
        
        public string SinisterNumber { get; set; }
        
        public string ReportNumber { get; set; }
        
        public DateTime SinisterDate { get; set; }
        
        public DateTime FailureDate { get; set; }
        
        public string FailureNumber { get; set; }
        
        public DateTime ShippingDate { get; set; }
        
        public DateTime ElaborationDate { get; set; }
        
        [StringLength(300)]
        public string Observation { get; set; }

        [StringLength(300)]
        public string Diagnosis { get; set; }
        
        public int UserId { get; set; }
        
        public int TypeDescriptionId { get; set; }
        
        public int AgremmentInfoId { get; set; }

        public int DTCStatus { get; set; }

        public bool Flag { get; set; }

        public bool OpenFlag { get; set; }

        public string SquareId { get; set; }

        public int AdminId { get; set; }

        public string DiagnosisReference { get; set; }
        #endregion
    }
}