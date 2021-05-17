using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Utilities
{
    public class NuevoUsuario
    {
        public string SqlResult { get; set; }
        public string SqlMessage { get; set; }
        public int UserId { get; set; }
        public string Pass { get; set; }
        public string UserName { get; set; }
    }
}