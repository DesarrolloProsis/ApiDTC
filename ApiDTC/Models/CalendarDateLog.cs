namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CalendarDateLog
    {
        public int CalendarId { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }

        [StringLength(30)]
        public string ReferenceNumber { get; set; }

        [StringLength(300)]
        public string Comment { get; set; }
    }
}