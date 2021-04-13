using System;

namespace ApiDTC.Data
{
    public class DiagnosisSheetView
    {
        public string ReferenceNumber { get; set; }

        public string SquareName { get; set; }

        public DateTime DiagnosisDate { get; set; }

        public string Lanes { get; set; }

        public string FailureNumber { get; set; }

        public string SinisterNumber { get; set; }

        public string Validacion { get; set; }
    }
}