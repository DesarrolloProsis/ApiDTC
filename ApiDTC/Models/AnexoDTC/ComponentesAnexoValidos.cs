﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class ComponentesAnexoValidos
    {
        public int RequestedComponentId { get; set; }
        public string NameComponent { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
    }    
}
