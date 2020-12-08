

namespace ApiDTC.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ActividadCalendario
    {
        public string[] CapufeLaneNums { get; set; }

        public string[] IdGares { get; set; }

        public string SquareId { get; set; }

        public int UserId { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public int FrequencyId { get; set; }

        public bool FinalFlag { get; set; }

        [StringLength(300)]
        public string Comment { get; set; }
    }

 

}
