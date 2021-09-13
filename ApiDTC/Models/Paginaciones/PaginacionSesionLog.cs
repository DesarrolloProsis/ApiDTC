using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class PaginacionSesionLog
    {
        public int NumeroPaginas { get; set; }

        public int PaginaActual { get; set; }

        public List<InfoTable> RowSesionLog { get; set; }
     
    }


    public class InfoTable 
    {
        public Int64 FIlaIndex { get; set; }
                
        public string Name { get; set; }

        public string RollDescription { get; set; }

        public DateTime DateStart { get; set; }
    }
}
