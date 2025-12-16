using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class EditListItem
    {
        [Required(ErrorMessage = "Enter a valid item")]
        public string Text { get; set; }

        public long Id { get; set; }

        public string Url { get; set; }
    }
}
