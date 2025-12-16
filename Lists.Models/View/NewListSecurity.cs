using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lists.Models.Data;

namespace Lists.Models.View
{
    public class NewListSecurity
    {
        [Required(ErrorMessage = "Enter a valid person")]
        public long PersonId { get; set; }

        public long ListId { get; set; }

        [Required(ErrorMessage = "Select a valid security type")]
        public ListSecurityType AccessType { get; set; }
    }
}
