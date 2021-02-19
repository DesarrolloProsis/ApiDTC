namespace ApiDTC.Models
{
    using System.ComponentModel.DataAnnotations;
    
    public class FichaTecnicaIntervencion
    {
        public int DiagnosisId { get; set; }

        public int TypeFaultId { get; set; }

        [StringLength(300)]
        public string Intervention { get; set; }
    }
}