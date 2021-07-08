using System;

namespace ApiDTC.Models
{
    public class DTCNoSellado
    {
        public string ReferenceNumber { get; set; }
        public int StatusId { get; set; }
        public DateTime FechaIngreso { get; set; }
    }
}
