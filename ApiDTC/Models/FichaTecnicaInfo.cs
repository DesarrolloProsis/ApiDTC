namespace ApiDTC.Models
{
    using System;
    
    public class FichaTecnicaInfo
    {        
        public string NumeroReporte { get; set; }

        public string PlazaCobro { get; set; }
        
        public string Ubicacion { get; set; }

        public DateTime Fecha { get; set; }

        public string Inicio { get; set; }

        public string Fin { get; set; }

        public string FolioFalla { get; set; }

        public string NumeroSiniestro { get; set; }

        public string DescripcionFalla { get; set; }

        public string Intervencion { get; set; }

        public int TipoFalloId { get; set; }

        public string TipoFallo { get; set; }

        public string AdministradorPlaza { get; set; }
    }
}