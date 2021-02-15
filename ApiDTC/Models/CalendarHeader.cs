namespace ApiDTC.Models
{
    using System;
    
    public class CalendarHeader
    {
        public string ReferenceNumber { get; set; }

        public string PlazaCobro { get; set; }

        public string Ubicacion { get; set; }

        public DateTime Fecha { get; set; }

        public string HoraInicio { get; set; }

        public string HoraFin { get; set; }

        public string Comentarios { get; set; }
    }
}