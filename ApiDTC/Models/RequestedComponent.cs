using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class RequestedComponent
    {
       
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
    }
}
