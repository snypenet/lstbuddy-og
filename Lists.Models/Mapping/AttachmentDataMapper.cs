using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class AttachmentDataMapper : 
        IMapper<SqlDataReader, AttachmentData>
    {
        public AttachmentData Convert(SqlDataReader source)
        {
            var attachmentData = new AttachmentData()
            {
                Data = source.GetSqlBytes(source.GetOrdinal("attachment_data")).Value,
                Id = source.GetInt64(source.GetOrdinal("attachment_data_id")),
                Mime = source.GetString(source.GetOrdinal("attachment_data_mime"))
            };

            return attachmentData;
        }
    }
}
