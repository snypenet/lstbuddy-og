using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class NewListItem
    {
        [Required(ErrorMessage = "Enter a valid item")]
        public string Text { get; set; }

        public long ListId { get; set; }

        public string Url { get; set; }
    }
}
