using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class ProfileInformation
    {
        public ExistingContactInformation ContactInformation { get; set; }
        public ExistingCredentials Credentials { get; set; }
        public List<QuickAddGroceryItem> QuickAddItems { get; set; }
        public List<QuickAddGroceryItem> AutoQuickAddItems { get; set; }
        public long PersonId { get; set; }
    }
}
