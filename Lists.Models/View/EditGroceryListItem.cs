using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class EditGroceryListItem : EditListItem
    {
        public EditGroceryListItem()
        {
            QuickAdd = new List<string>
            {
                "Milk",
                "Eggs",
                "Diapers", 
                "Apples", 
                "Bananas",
                "Butter",
                "Pop"
            };
        }

        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Select a valid quantity")]
        public int? Quantity { get; set; }

        public IEnumerable<string> QuickAdd { get; set; }
    }
}
