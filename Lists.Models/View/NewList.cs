using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class NewList
    {
        [Required(ErrorMessage="Please Select a list type")]
        public long ListType { get; set; }

        [Required(ErrorMessage = "Enter a valid list name")]
        public string Name { get; set; }

        public DateTime? ExpiresOn { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Optional, only for grocery lists
        /// </summary>
        public bool AutoPopulate { get; set; }
    }
}
