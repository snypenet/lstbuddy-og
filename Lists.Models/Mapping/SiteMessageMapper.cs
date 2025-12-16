using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class SiteMessageMapper :
        IMapper<SqlDataReader, SiteMessage>
    {
        public SiteMessage Convert(SqlDataReader source)
        {
            return new SiteMessage
            {
                Id = source.GetInt64(source.GetOrdinal("id")),
                ShowFrom = source.GetDateTime(source.GetOrdinal("show_from")),
                ShowTo = source.GetDateTime(source.GetOrdinal("show_to")),
                Text = source.GetString(source.GetOrdinal("text")),
                Type = (SiteMessageType)source.GetInt32(source.GetOrdinal("type"))
            };
        }
    }
}


