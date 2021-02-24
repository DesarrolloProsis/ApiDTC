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

        public int Day { get; set; }

        public int FrequencyId { get; set; }

        public DateTime DateStamp { get; set; }

        public int CalendarId { get; set; }

        public bool StatusMaintenance { get; set; }

        public string StatusMaintenance { get; set; }

    }
}
