namespace ApiDTC.Models
{
    using System;
    public class Bitacora
    {
        public string Fecha { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string SquareName { get; set; }

        public string DelegationName { get; set; }

        public int DelegationId { get; set; }

        public string PlazaId { get; set; }

        public string Nombre { get; set; }

        public int UserId { get; set; }
    }
}