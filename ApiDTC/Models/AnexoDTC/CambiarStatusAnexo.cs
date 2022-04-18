using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.AnexoDTC
{
    public class CambiarStatusAnexo
    {
        public int userId { get; set; }
        public string anexoReference { get; set; }
        public int statusId { get; set; }
        public string comentario { get; set; }
    }
}
