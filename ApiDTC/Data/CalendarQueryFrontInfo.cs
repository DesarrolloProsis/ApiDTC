using ApiDTC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class CalendarQueryFrontInfo
    {
        public string Lane { get; set; }

        public string CapufeLaneNum { get; set; }

        public string IdGare { get; set; }

        public int Day { get; set; }

        public int FrequencyId { get; set; }

        public DateTime DateStamp { get; set; }

        public int CalendarId { get; set; }

        public bool StatusMaintenance { get; set; }
        public int AdminId { get; set; }
        public string SquareId {get; set;}

        public string ReferenceNumber { get; set; }

        public bool PdfExists { get; set; }
    }
}
