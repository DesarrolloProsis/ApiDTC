namespace ApiDTC.Models
{
    using System;
    public class RequestedComponent
    {
        #region Properties
        /* Versión no libre
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
        
        public int IntPartida { get; set; }*/

        //MODO LIBRE
        public int IntType { get; set; }

        public bool BitFlag { get; set; }

        public string StrReferenceNumber { get; set; }

        public string StrUnity { get; set; }

        public string StrComponent { get; set; }

        public string StrQuantity { get; set; }

        public string StrBrand { get; set; }

        public string StrModel { get; set; }

        public string StrBrandProposed { get; set; }

        public string StrModelProposed { get; set; }

        public string StrSerialNumber { get; set; }

        public string StrLane { get; set; }

        public string StrInstallationDate { get; set; }

        public string StrMaintenanceDate { get; set; }

        public string StrLifeTimeExpected { get; set; }

        public int IntItem { get; set; }

        public string StrMaintenanceFolio { get; set; }

        public string StrLifeTimeReal { get; set; }

        public string StrUnitaryPrice { get; set; }

        public string StrDollarUnitaryPrice { get; set; }

        public string StrTotalPrice { get; set; }

        public string StrDollarTotalPrice { get; set; }
        #endregion
    }
}
