namespace ApiDTC.Models
{
    using System;

    public class Components
    {
        #region Properties
        public string Unity { get; set; }
        
        public string Description { get; set; }
        
        public string Brand { get; set; }
        
        public string Model { get; set; }
        
        public string SerialNumber { get; set; }
        
        public DateTime InstalationDate { get; set; }
        
        public int LifeTime { get; set; }
        
        public string Lane { get; set; }
        
        public string IdGare { get; set; }
        
        public string CapufeLaneNum { get; set; }
        
        public int ComponentsStockId { get; set; }
        
        public decimal UnitaryPrice { get; set; }
        
        public bool SelfAssignable { get; set; }
        
        public bool VitalComponent { get; set; }
        
        public string CatalogBrand { get; set; }
        
        public string CatalogModel { get; set; }
        #endregion
    }
}
