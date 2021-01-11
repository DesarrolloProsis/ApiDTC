using System.ComponentModel.DataAnnotations;

namespace ApiDTC.Models
{
    public class CalendarActivity
    {
        [StringLength(500)]
        public string ReferenceNumber { get; set; }

        public int ComponentJob { get; set; }

        public int JobStatus { get; set; }
    }
}
