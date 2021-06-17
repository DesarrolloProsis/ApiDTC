using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDTC.Models
{
    public class Comment
    {
        public string TextoComment { get; set; }
        public int CommentId { get; set; }
        public int UserId { get; set; }

    }
}
