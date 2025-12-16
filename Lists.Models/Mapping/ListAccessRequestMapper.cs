using Lists.Models.Data;
using Lists.Models.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Mapping
{
    public class ListAccessRequestMapper 
        : IMapper<SqlDataReader, ListAccessRequest>
    {
        public ListAccessRequest Convert(SqlDataReader source)
        {
            return new ListAccessRequest
            {
                AccessType = (ListSecurityType)((int)source["access_type"]),
                Id = (int)source["id"],
                List = new List((long)source["list_id"]),
                Requestor = new Person((long)source["person_id"])
            };
        }
    }
}
