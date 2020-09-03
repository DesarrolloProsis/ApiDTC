using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class RequestedComponentOpen
    {
        #region Properties
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
