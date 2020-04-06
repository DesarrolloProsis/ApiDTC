namespace ApiDTC.Models
{
    using System;
    public class RequestedComponent
    {
        #region Properties
        public int ComponentsStockId { get; set; }
        
        public string ReferenceNumber { get; set; }
        
        public string[] CapufeLaneNum { get; set; }
        
        public string[] IdGare { get; set; }            
        
        public string Marca { get; set; }
        
        public string Modelo { get; set; }
        
        public string[] NumSerie { get; set; }
        
        public string Unity { get; set; }
        
        public DateTime dateInstallationDate { get; set; }
        
        public DateTime dateMaintenanceDate { get; set; }
        
        public int intLifeTimeExpected { get; set; }
        
        public DateTime dateLifeTimeReal { get; set; }
        
        public int intPartida { get; set; }
        #endregion
    }
}
