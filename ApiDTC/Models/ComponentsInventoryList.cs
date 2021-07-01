using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class ComponentsInventoryList
    {
        public string Lane { get; set; }

        public string Component { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public string InstallationDate { get; set; }

        public string MaintenanceDate { get; set; }
        
        public string MaintenanceFolio { get; set; }
        
        public int TableFolio { get; set; }
        public bool VitalComponent { get; set; }
        public int ComponentsRelationShip { get; set; }
    }
}
