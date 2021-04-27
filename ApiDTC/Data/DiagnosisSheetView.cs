using System;

namespace ApiDTC.Data
{
    public class DiagnosisSheetView
    {
        public string ReferenceNumber { get; set; }

        public string SquareName { get; set; }

        public DateTime DiagnosisDate { get; set; }

        public string Lanes { get; set; }

        public string FailuerNumber { get; set; }

        public string SiniesterNumber { get; set; }

        public string Validacion { get; set; }

        public string SquareId { get; set; }

        public string CauseFailure { get; set; }

        public string FailureDescription { get; set; }

        public string FailureDiagnosis { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public int AdminSquareId { get; set;}
    }
}