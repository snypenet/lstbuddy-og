using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class ResetPatternControl
    {
        [Required(ErrorMessage="Select a pattern")]
        public string Pattern { get; set; }

        [Required(ErrorMessage="Enter your email")]
        public string Email { get; set; }

        [Required(ErrorMessage="Enter your username")]
        public string Username { get; set; }

        public string ControlId { get; set; }
    }
}
