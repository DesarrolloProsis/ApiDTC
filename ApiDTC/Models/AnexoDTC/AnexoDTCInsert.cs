using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.AnexoDTC
{
    public class AnexoDTCInsert
    {
        public string DTCReference { get; set; }
        public string AnexoReference { get; set; }
        public string Solicitud { get; set; }
        public DateTime? FechaSolicitudInicio { get; set; }
        //public DateTime? FechaSolicitudFin { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string FolioOficio { get; set; }
        public DateTime? FechaOficioInicio { get; set; }
        //public DateTime? FechaOficioFin { get; set; }
        //public int SupervisorId { get; set; }
        public string Observaciones { get; set; }
        public int Testigo1Id { get; set; }
        public int Testigo2Id { get; set; }
        public char TipoAnexo { get; set; }
        public List<ComponetesAnexoInsert> ComponentesAnexo { get; set; }
    }
    public class ComponetesAnexoInsert 
    {
        public int RequestedComponentId { get; set; }
        public string SerialNumber { get; set; }
    }
}
