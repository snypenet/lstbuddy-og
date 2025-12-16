using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class EditProsAndConsListItem : EditListItem
    {
        public ProConWeight Weight { get; set; }
        public ProOrCon Type { get; set; }
    }
}
