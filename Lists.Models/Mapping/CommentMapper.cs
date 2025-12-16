using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class CommentMapper : 
        IMapper<SqlDataReader, Comment>
    {
        public Comment Convert(SqlDataReader source)
        {
            var comment = new Comment()
            {
                CreatedBy = new Person(source.GetInt64(source.GetOrdinal("created_by"))),
                Id = source.GetInt64(source.GetOrdinal("comment_id")),
                Text = source.GetString(source.GetOrdinal("text")),
                ListItemId = source.GetInt64(source.GetOrdinal("list_item_id")),
                CreatedOn = source.GetDateTime(source.GetOrdinal("created_on")),
                IsWhisper = source.GetBoolean(source.GetOrdinal("is_whisper"))
            };

            return comment;
        }
    }
}
