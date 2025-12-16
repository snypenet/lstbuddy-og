using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;


namespace Lists.Models.Mapping
{
    public class ListMapper : 
        IMapper<SqlDataReader, List>
    {
        private ListTypeMapper listTypeMapper;

        public ListMapper()
        {
            listTypeMapper = new ListTypeMapper();
        }

        public List Convert(SqlDataReader source)
        {
            var list = new List()
            {
                CreatedBy = new Person(source.GetInt64(source.GetOrdinal("created_by"))),
                CreatedOn = source.GetDateTime(source.GetOrdinal("created_on")),
                Name = source.GetString(source.GetOrdinal("list_name")),
                Owner = new Person(source.GetInt64(source.GetOrdinal("owner_id"))),
                Type = new ListType(source.GetInt64(source.GetOrdinal("list_type"))),
                Id = source.GetInt64(source.GetOrdinal("list_id")),
                IsPublic = source.GetBoolean(source.GetOrdinal("is_public")),
                IsPrivate = source.GetBoolean(source.GetOrdinal("is_private"))
            };


            if (!source.IsDBNull(source.GetOrdinal("expires_on")))
            {
                list.ExpiresOn = source.GetDateTime(source.GetOrdinal("expires_on"));
            }

            if (source.IsDBNull(source.GetOrdinal("share_id")))
            {
                list.ShareId = Guid.NewGuid().ToString();
                list.Update(); //save the new share id
            }
            else
            {
                list.ShareId = source.GetString(source.GetOrdinal("share_id"));
            }
            
            return list;
        }
    }
}
