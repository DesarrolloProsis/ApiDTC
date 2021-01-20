namespace ApiDTC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CalendarReportData
    {
        [StringLength(30)]
        public string ReferenceNumber { get; set; }

        [StringLength(4)]
        public string SquareId { get; set; }

        [StringLength(10)]
        public string CapufeLaneNum { get; set; }

        [StringLength(3)]
        public string IdGare { get; set; }

        public int UserId { get; set; }

        public int AdminSquare { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        [StringLength(30)]
        public string Start { get; set; }

        [StringLength(30)]
        public string End { get; set; }

        [StringLength(500)]
        public string Observations { get; set; }
    }
}
