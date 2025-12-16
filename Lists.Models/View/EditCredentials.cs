using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class EditCredentials
    {
        [Required(ErrorMessage="Enter a username")]
        public string Username { get; set; }

        [Required(ErrorMessage="Select a Pattern")]
        public string Pattern { get; set; }
    }
}
