using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lists.Models.Data;
using Lists.Constants;
using System.Data.SqlClient;

namespace Lists.Models.Mapping
{
    public class ListTypeMapper :
        IMapper<SqlDataReader, ListType>
    {
        public ListType Convert(SqlDataReader source)
        {
            return new ListType()
            {
                Id = source.GetInt64(source.GetOrdinal("list_type_id")),
                Name = source.GetString(source.GetOrdinal("list_type"))
            };
        }
    }
}
