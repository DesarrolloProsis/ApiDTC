using ApiDTC.Models.DTCGMMEP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.Paginaciones
{
    public class PaginacionDTCGMMEP
    {
        public int NumeroPaginas { get; set; }

        public int PaginaActual { get; set; }

        public List<DTCHeaderPaginacionView> Rows { get; set; }
    }
}
