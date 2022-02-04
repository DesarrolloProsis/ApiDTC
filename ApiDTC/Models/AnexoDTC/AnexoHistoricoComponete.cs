using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.AnexoDTC
{
    public class AnexoHistoricoComponete
    {
        public int Id { get; set; }
        public string AnexoId { get; set; }
        public int ComponentDTCId { get; set; }
        public string NumeroSerie { get; set; }
    }
}
