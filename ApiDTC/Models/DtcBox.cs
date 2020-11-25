using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class DtcBox
    {
        public string ComponentePrincipal { get; set; }

        public List<string> Secundarios { get; set; }

        public int ComponentsRelationship { get; set; }

        public int NumberOfComponents { get; set; }

    }
}
