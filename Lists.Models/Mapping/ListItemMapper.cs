using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Lists.Models.Data;

namespace Lists.Models.Mapping
{
    public class ListItemMapper :
        IMapper<SqlDataReader, ListItem>
    {
        public virtual ListItem Convert(SqlDataReader source)
        {
            var listItem = new ListItem()
            {
                CreatedBy = new Person(source.GetInt64(source.GetOrdinal("created_by"))),
                CreatedOn = source.GetDateTime(source.GetOrdinal("created_on")),
                LastModifiedby = new Person(source.GetInt64(source.GetOrdinal("last_modified_by"))),
                Text = source.GetString(source.GetOrdinal("list_item_content")),
                LastModifiedOn = source.GetDateTime(source.GetOrdinal("last_modified_on")),
                Id = source.GetInt64(source.GetOrdinal("list_item_id")),
                ListId = source.GetInt64(source.GetOrdinal("list_id"))
            };

            if (!source.IsDBNull(source.GetOrdinal("grocery_price")))
            {
                listItem.Price = source.GetDecimal(source.GetOrdinal("grocery_price"));
            }

            if (!source.IsDBNull(source.GetOrdinal("quantity")))
            {
                listItem.Quantity = source.GetInt32(source.GetOrdinal("quantity"));
            }

            if(!source.IsDBNull(source.GetOrdinal("purchased")))
            {
                listItem.Purchased = source.GetBoolean(source.GetOrdinal("purchased"));
            }

            if(!source.IsDBNull(source.GetOrdinal("prayer_count")))
            {
                listItem.PrayerCount = source.GetInt64(source.GetOrdinal("prayer_count"));
            }

            if(!source.IsDBNull(source.GetOrdinal("url")))
            {
                listItem.Url = source.GetString(source.GetOrdinal("url"));
            }

            if (!source.IsDBNull(source.GetOrdinal("pro_con_type")))
            {
                listItem.ProConType = (ProOrCon)source.GetInt32(source.GetOrdinal("pro_con_type"));
            }

            if (!source.IsDBNull(source.GetOrdinal("pro_con_weight")))
            {
                listItem.Weight = (ProConWeight)source.GetInt32(source.GetOrdinal("pro_con_weight"));
            }

            return listItem;
        }
    }
}


