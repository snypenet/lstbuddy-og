using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class NewPerson 
    {
        [Required(ErrorMessage = "Enter a valid pattern")]
        public string Pattern { get; set; }

        [Required(ErrorMessage = "Enter a valid username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Enter a valid E-Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter a valid first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter a valid last name")]
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string CellPhoneNumber { get; set; }

        public long? CellPhoneProvider { get; set; }
    }
}
