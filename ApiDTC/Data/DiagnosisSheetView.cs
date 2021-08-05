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

        public bool ValidacionFichaTecnica { get; set; }
        //public string Validacion { get; set; }

        public string SquareId { get; set; }

        public string CauseFailure { get; set; }

        public string FailureDescription { get; set; }

        public string FailureDiagnosis { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public int AdminSquareId { get; set; }

        public string Intervention { get; set; }

        public string FaultDescription { get; set; }

        public int TypeFaultId { get; set; }
        public bool ValidacionDTC { get; set; }
        public string ReferenceDTC { get; set; }
             
    }
    public class DiagnosisSheetViewValid
    {
        public string ReferenceNumber { get; set; }

        public string SquareName { get; set; }

        public DateTime DiagnosisDate { get; set; }

        public string Lanes { get; set; }

        public string FailuerNumber { get; set; }

        public string SiniesterNumber { get; set; }

        public bool ValidacionFichaTecnica { get; set; }
        //public string Validacion { get; set; }

        public string SquareId { get; set; }

        public string CauseFailure { get; set; }

        public string FailureDescription { get; set; }

        public string FailureDiagnosis { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public int AdminSquareId { get; set; }

        public string Intervention { get; set; }

        public string FaultDescription { get; set; }

        public int TypeFaultId { get; set; }
        public bool ValidacionDTC { get; set; }
        public string ReferenceDTC { get; set; }
           public bool DiagnosticoSellado { get; set; }
        public bool FichaSellado { get; set; }
        
    }
}