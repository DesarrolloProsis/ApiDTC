namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DiagnosticoDeFallaInfo
    {
        public string NumeroReporte { get; set; }

        public string PlazaCobro { get; set; }

        public string Ubicacion { get; set; }
        
        public DateTime Fecha { get; set; }
        
        public string Inicio { get; set; }
        
        public string Fin { get; set; }

        public string FolioFalla { get; set; }
        
        public string NumeroSiniestro { get; set; }

        public string TecnicoProsis { get; set; }

        public string DescripcionFalla { get; set; }

        public string DiagnosticoFalla { get; set; }

        public string CausaFalla { get; set; }

        public string AdministradorPlaza { get; set; }

        public int UserId { get; set; }
    }
}