using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class DtcFechas
    {
        public string Reference { get; set; }
        public DateTime SinisterDate { get; set; }
        public DateTime FailureDate { get; set; }

        public DateTime ShippingDate { get; set; }
        public DateTime ElaborationDate { get; set; }

    }
}
