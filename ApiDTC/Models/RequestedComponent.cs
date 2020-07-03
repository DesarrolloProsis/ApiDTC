namespace ApiDTC.Models
{
    using System;
    public class RequestedComponent
    {
        #region Properties
        public int ComponentsStockId { get; set; }
        
        public string ReferenceNumber { get; set; }
        
        public string CapufeLaneNum { get; set; }
        
        public string IdGare { get; set; }            
        
        public string Marca { get; set; }
        
        public string Modelo { get; set; }
        
        public string NumSerie { get; set; }
        
        public string Unity { get; set; }
        
        public DateTime DateInstallationDate { get; set; }
        
        public DateTime DateMaintenanceDate { get; set; }

        public string MaintenanceFolio { get; set; }
        
        public int IntLifeTimeExpected { get; set; }
        
        public string strLifeTimeReal { get; set; }
        
        public int IntPartida { get; set; }

        #endregion
    }
}
