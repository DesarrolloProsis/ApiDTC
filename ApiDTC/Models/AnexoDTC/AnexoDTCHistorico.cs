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
        public DateTime FechaCierre { get; set; }
        public int SupervisorId { get; set; }
    }

    public class AnexoCount{
        public string AnexoReference { get; set; }
    }
}
