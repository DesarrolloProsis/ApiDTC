using System;

namespace ApiDTC.Models
{
    public class CalendarQueryFront
    {
        public string Lane { get; set; }

        public string CapufeLaneNum { get; set; }

        public string IdGare { get; set; }

        public int Day { get; set; }

        public int FrequencyId { get; set; }

        public DateTime DateStamp { get; set; }

        public int CalendarId { get; set; }

        public bool StatusMaintenance { get; set; }

        public string ReferenceNumber { get; set; }
    }
}
