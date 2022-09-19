using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models.Email
{
    public class Email
    {
        public string addressee { get; set; }
        public string affair { get; set; }
        public string body { get; set; }
    }
}
