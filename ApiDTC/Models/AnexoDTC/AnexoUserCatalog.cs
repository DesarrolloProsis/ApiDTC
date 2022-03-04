using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.AnexoDTC
{
    public class AnexoUsuarioPlaza
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string SquareId { get; set; }
        public int RollId { get; set; }
    }
    public class InsertUsuarioAnexo
    {
        public string Nombre { get; set; }
        public string SquareId { get; set; }
    }
}
