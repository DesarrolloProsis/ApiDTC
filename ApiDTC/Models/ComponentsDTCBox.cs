using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class ComponentsDTCBox
    {

        public string Description { get; set; }

        public int AttachedId { get; set; }

        public int ComponentsRelationship { get; set; }

        public bool VitalComponent { get; set; }

        public int ComponentsStockId { get; set; }



    }
}
