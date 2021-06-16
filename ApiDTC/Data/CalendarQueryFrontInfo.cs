using ApiDTC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Data
{
    public class CalendarQueryFrontInfo
    {
        public CalendarQueryFront CalendarQueryFront { get; set; }

        public bool PdfExists { get; set; }
    }
}
