using System;

namespace ApiDTC.Models
{
    public class Autorizacion
    {
        public int AutoriId { get; set; }
        public string LabelAutorizacion { get; set; }
        public string NameAutorizacion { get; set; }
        public string FirmaImagen { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public int Activo { get; set; }
    }
}