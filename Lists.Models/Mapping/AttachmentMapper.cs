using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class AttachmentMapper : 
        IMapper<SqlDataReader, Attachment>
    {
        public Attachment Convert(SqlDataReader source)
        {
            var attachment = new Attachment()
            {
                AttachmentType = source.GetInt64(source.GetOrdinal("attachment_type")),
                Id = source.GetInt64(source.GetOrdinal("attachment_id")),
                Description = source.GetString(source.GetOrdinal("attachment_description")),
                Data = new AttachmentData(source.GetInt64(source.GetOrdinal("attachment_data_id"))),
                ListItemId = source.GetInt64(source.GetOrdinal("list_item_id"))
            };

            return attachment;
        }
    }
}
