using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class CalendarQuery
    {
        public string Lane { get; set; }

        public string CapufeLaneNum { get; set; }

        public string IdGare { get; set; }

        public string Day { get; set; }

        public string FrequencyId { get; set; }

        public string DateStamp { get; set; }

        public string CalendarId { get; set; }

        public string StatusMaintenance { get; set; }

    }
}
