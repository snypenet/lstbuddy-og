using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class EditListAccessRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Select a list access type")]
        public ListSecurityType SecurityType { get; set; }
    }
}
