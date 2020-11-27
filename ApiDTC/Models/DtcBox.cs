using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class DtcBox
    {
        public string ComponentePrincipal { get; set; }

        public List<ComponentDtcBoxPrincipal> Secundarios { get; set; }
    }
}
