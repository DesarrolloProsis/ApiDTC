namespace ApiDTC.Models
{
    using System.ComponentModel.DataAnnotations;
    public class InsertAdmin
    {
        [StringLength(30)]
        public string Nombre { get; set; }

        [StringLength(20)]
        public string ApellidoP { get; set; }   

        [StringLength(20)]
        public string ApellidoM { get; set; }   

        [StringLength(30)]
        public string Mail { get; set; }

        [StringLength(4)]
        public string Plaza { get; set; }
    }
}