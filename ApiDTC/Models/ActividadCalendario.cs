

namespace ApiDTC.Models
{
    using System.Collections.Generic;

    public class ActividadCalendario
    {
        public List<string> CapufeLaneNums { get; set; }

        public List<string> IdGares { get; set; }

        public string SquareId { get; set; }

        public int UserId { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public int FrequencyId { get; set; }

        public bool FinalFlag { get; set; }

        public string Comment { get; set; }
    }
}
