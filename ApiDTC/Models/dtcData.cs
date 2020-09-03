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
        
        public string Observation { get; set; }
        
        public string Diagnosis { get; set; }
        
        public int UserId { get; set; }
        
        public int TypeDescriptionId { get; set; }
        
        public int AgremmentInfoId { get; set; }

        public int DTCStatus { get; set; }

        public bool Flag { get; set; }

        public bool OpenFlag { get; set; }
        #endregion
    }

    
}