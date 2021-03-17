namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    public class FichaTecnica
    {
        public int TypeFaultId { get; set; }
        
        [StringLength(30)]
        public string ReferenceNumber { get; set; }

        [StringLength(300)]
        public string Intervention { get; set; }

        public bool UpdateFlag { get; set; }
    }
}