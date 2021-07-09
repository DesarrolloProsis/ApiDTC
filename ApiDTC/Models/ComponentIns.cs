using System;

namespace ApiDTC.Models
{
    public class ComponentIns
    {
        public string CapufeNum { get; set; }
        public string IdGare { get; set; }
        public string InventaryNumCapufe { get; set; }
        public string InventaryNumProsis { get; set; }
        public string Component { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstalationDate { get; set; }
        public string Observations { get; set; }
        public int TypeUbication { get; set; }
        public string Brand { get; set; }
        public string Replace { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public int MaintenanceFolio { get; set; }
        public int AttahcedId { get; set; }
    }
}
