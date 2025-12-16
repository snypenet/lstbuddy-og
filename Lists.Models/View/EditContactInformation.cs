using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class EditContactInformation
    {
        [Required(ErrorMessage="Enter an email address")]
        public string Email { get; set; }

        public string CellPhoneNumber { get; set; }
        public long? CellPhoneProvider { get; set; }
    }
}
