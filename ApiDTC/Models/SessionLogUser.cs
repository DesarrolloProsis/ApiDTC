using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class SessionLogUser
    {
        public Int64 FIlaIndex { get; set; }
        
        public Int64  FilaIndexAsc { get; set; }

        public string Name { get; set; }

        public string RollDescription { get; set; }

        public DateTime DateStart { get; set; }
    }
}
