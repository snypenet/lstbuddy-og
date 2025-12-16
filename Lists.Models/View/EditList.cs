using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class EditList
    {
        [Required(ErrorMessage = "Please Select a list type")]
        public long ListType { get; set; }

        public DateTime? ExpiresOn { get; set; }
        public long Id { get; set; }

        [Required(ErrorMessage = "Enter a valid name")]
        public string Name { get; set; }

        public bool IsPublic { get; set; }
        public bool IsPrivate { get; set; }
    }
}
