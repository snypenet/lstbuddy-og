using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lists.Models.Data;

namespace Lists.Models.Data
{
    public class ListSecurity
    {
        public Person Person { get; set; }

        public long ListId { get; set; }

        public ListSecurityType Access { get; set; }
    }
}
