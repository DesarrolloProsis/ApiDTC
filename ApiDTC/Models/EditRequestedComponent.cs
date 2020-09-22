namespace ApiDTC.Models
{
    public class EditRequestedComponent
    {
        public int Item { get; set; }

        public string Unity { get; set; }

        public string Component { get; set; }

        public string Quantity { get; set; }

        public string Brand { get; set; }

        public string Model{ get; set; }

        public string SerialNumber{ get; set; }

        public string Lane { get; set; }

        public string InstallationDate { get; set; }

        public string MaintenanceDate { get; set; }

        public string MaintenanceFolio { get; set; }

        public string LifeTimeReal { get; set; }

        public string LifeTimeExpected { get; set; }
    }
}
