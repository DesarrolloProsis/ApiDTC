
namespace ApiDTC.Models
{
    using System.Collections.Generic;
    public class Equipo
    {
        public string Nombre { get; set; }

        public List<Componente> Componentes { get; set; }
    }
}