using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class HeaderEditDTC
    {

        public string Nombre { get; set; }
        
        public string Plaza { get; set; }
        
        public string Agrement { get; set; }
        
        public string ManagerName { get; set; }
        
        public string Mail { get; set; }
        
        public DateTime AgremmentDate { get; set; }
        
        public string DelegationName { get; set; }

        public int UserId { get; set; }

        public int AgremmentInfoId { get; set; }

        public string RegionalCoordination { get; set; }

        public string ReferenceSquare  { get; set; }

        public string AdminName { get; set; }

        public string AdminMail { get; set; }

        public int AdminSquareId { get; set; }
    }
}
