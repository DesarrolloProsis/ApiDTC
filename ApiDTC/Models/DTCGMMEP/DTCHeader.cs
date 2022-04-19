using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.DTCGMMEP
{
    public class DTCHeader
    {
        public string ReferenceNumber { get; set; }
        public int AdminId { get; set; }

        public string UserName { get; set; }

        public string SinisterNumber { get; set; }

        public string ReportNumber { get; set; }

        public DateTime SinisterDate { get; set; }

        public int StatusId { get; set; }

        public DateTime FailureDate { get; set; }

        public string FailureNumber { get; set; }

        public DateTime ShippingDate { get; set; }

        public DateTime ElaborationDate { get; set; }

        public DateTime DateStamp { get; set; }

        public int TypeDescriptionId { get; set; }

        public string TypeDescription { get; set; }

        public string Observation { get; set; }

        public string Diagnosis { get; set; }

        public string StatusDescription { get; set; }

        public bool OpenMode { get; set; }

        public string SquareCatalogId { get; set; }

        public string ReferenceSquare { get; set; }

        public int UserId { get; set; }
        public int TypeFaultId { get; set; }
        public string TechnicalSheetReference { get; set; }
        public string FaultDescription { get; set; }
        public bool IsAnexoCreate { get; set; }
        public bool IsValidCreate { get; set; }
        public string Name { get; set; }
    }

    public class DTCHeaderPaginacion : DTCHeader
    {
        public Int64 FIlaIndex { get; set; }
        public Int64 FilaIndexAsc { get; set; }
    }
    public class DTCHeaderPaginacionView : DTCHeader
    {
        public Int64 FIlaIndex { get; set; }        
    }
}
