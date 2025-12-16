using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.View
{
    public class ExistingContactInformation
    {
        public string Email { get; set; }
        public string CellPhoneNumber { get; set; }
        public CellPhoneProvider CellPhoneProvider { get; set; }
    }
}
