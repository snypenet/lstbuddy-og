using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Mapping
{
    public class ChangeControlMapper :
        IMapper<SqlDataReader, ChangeControl>
    {
        public ChangeControl Convert(SqlDataReader source)
        {
            return new ChangeControl
            {
                Id = source.GetString(source.GetOrdinal("change_id")),
                Used = source.GetBoolean(source.GetOrdinal("used"))
            };
        }
    }
}
