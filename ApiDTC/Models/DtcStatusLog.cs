using System;
using System.ComponentModel.DataAnnotations;

namespace ApiDTC.Models
{
    public class DtcStatusLog
    {
        public string ReferenceNumber { get; set; }

        public int StatusId { get; set; }

        public string Nombre { get; set; }

        public int UserId { get; set; }

        public string Comment { get; set; }

        public DateTime DateStamp { get; set; }

        public int StatusID_OLD { get; set; }

        public int UserId_OLD { get; set; }

        public bool Viewed { get; set; }
    }
    public class DtcStatusLogView
    {
        public string ReferenceNumber { get; set; }

        public int StatusId { get; set; }

        public string Nombre { get; set; }

        public DateTime DateStamp { get; set; }

        public bool Viewed { get; set; }

        public int UserId { get; set; }
    }
}
