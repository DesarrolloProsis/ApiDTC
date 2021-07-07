using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class ReporteComponente
    {
        public string Plaza { get; set; }//
        public string Carril { get; set; }//
        public string Componente { get; set; }//
        public int Cantidad { get; set; }
        public Decimal Precio { get; set; }//
        public string Solicitante { get; set; }//
        public string TipoDTC { get; set; }//
        public string Referencia { get; set; }//
    }
}
