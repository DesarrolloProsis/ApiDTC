using System.ComponentModel.DataAnnotations;

namespace ApiDTC.Models
{
    public class DtcStatusLog
    {
        [Required]
        [StringLength(20)]
        public string ReferenceNumber { get; set; }

        public int StatusId { get; set; }

        public int UserId { get; set; }

        [StringLength(300)]
        public string Comment { get; set; }
    }
}
