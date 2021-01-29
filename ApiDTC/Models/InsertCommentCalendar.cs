using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class InsertCommentCalendar
    {

        public int UserId { get; set; }

        public int Month { get; set; }

        [StringLength(600)]
        public string Comment { get; set; }

        public string SquareId { get; set; }

        public int Year { get; set; }
    }
}
