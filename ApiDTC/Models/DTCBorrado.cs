using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class DTCBorrado
    {
        public string ReferenceNumber { get; set; }
        public int Conteos { get; set; }
        public DateTime UltimaFecha { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
    }
}
