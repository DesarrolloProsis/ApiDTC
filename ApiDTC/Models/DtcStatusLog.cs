using System.ComponentModel.DataAnnotations;

namespace ApiDTC.Models
{
    public class DtcStatusLog
    {
        public string ReferenceNumber { get; set; }

        public int StatusId { get; set; }

        public int UserId { get; set; }

        public string Comment { get; set; }
    }
}
