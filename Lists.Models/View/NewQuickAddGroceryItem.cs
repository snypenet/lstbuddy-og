using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class NewQuickAddGroceryItem
    {
        [Required(ErrorMessage="Item name is required")]
        public string ItemName { get; set; }
    }
}
