using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.AnexoDTC
{
    public class AnexoDTCHistorico
    {
        public string DTCReference { get; set; }
        public string AnexoReference { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Solicitud { get; set; }
        public DateTime? FechaSolicitudInicio { get; set; }
        //public DateTime? FechaSolicitudFin { get; set; }
        public string FolioOficio { get; set; }
        public DateTime? FechaOficioInicio { get; set; }
        //public DateTime? FechaOficioFin { get; set; }
        public int Testigo1Id { get; set; }
        public int Testigo2Id { get; set; }
        public int SupervisorId { get; set; }
        public DateTime FechaUltimoCambio { get; set; }
        public string Comentarios { get; set; }
        public string TipoAnexo { get; set; }
        public bool Activo { get; set; }
        public bool IsSubVersion { get; set; }
        public bool PDFFirmardo { get; set; }
        public bool PDFFotografico { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        
    }
    public class HistoricoWhiFlag : AnexoDTCHistorico
    {
        public bool ComponentesLibres { get; set; }
    }
    public class HeaderAnexo : AnexoDTCHistorico
    {

    }
    public class AnexoCount{
        public string AnexoReference { get; set; }
    }
}
