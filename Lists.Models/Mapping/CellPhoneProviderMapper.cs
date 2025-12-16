using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class CellPhoneProviderMapper : 
        IMapper<SqlDataReader, CellPhoneProvider>
    {
        public CellPhoneProvider Convert(SqlDataReader source)
        {
            var provider = new CellPhoneProvider()
            {
                Name = source.GetString(source.GetOrdinal("provider_name")),
                Id = source.GetInt64(source.GetOrdinal("provider_id")),
                AddressFormat = source.GetString(source.GetOrdinal("provider_email_format"))
            };

            return provider;
        }
    }
}
