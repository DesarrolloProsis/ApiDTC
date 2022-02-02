using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class ComponentesAnexoValidos
    {
        public int Item { get; set; }
        public int RequestedComponentId { get; set; }
        public string SerialNumber { get; set; }
        public bool UseInAnexo { get; set; }
    }
}
