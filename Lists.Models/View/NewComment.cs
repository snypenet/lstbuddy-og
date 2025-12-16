using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class NewComment
    {
        [Required(ErrorMessage="Enter a comment")]
        public string CommentText { get; set; }
        public long ListItemId { get; set; }
        public bool IsParial { get; set; }
        public bool IsWhisper { get; set; }
    }
}
