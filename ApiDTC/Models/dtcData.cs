using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class DTCData
    {
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
    }
}
