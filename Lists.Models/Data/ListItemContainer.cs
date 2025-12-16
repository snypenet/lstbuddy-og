using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

using Lists.Constants;
using Lists.Models.Mapping;

namespace Lists.Models.Data
{
    public class ListItemContainer
    {
        public ListItemContainer Filter(Func<ListItem, bool> predicate)
        {
            var filtered = listItems.Where(predicate);
            var itemCollection = new ListItemContainer();
            itemCollection.ListId = this.ListId;
            itemCollection.Add(filtered);
            return itemCollection;
        }

        public void Add(IEnumerable<ListItem> items)
        {
            listItems.AddRange(items);
        }
        
        public ListItemContainer()
        {
        }

        public ListItemContainer(long listId)
        {
            this.Load(listId);
        }

        public void Load(long listId)
        {
            ListId = listId;
            this.listItems.Clear();
            var command = new SqlCommand(SQL.GetListItemsByListId);
            command.Parameters.AddWithValue("@list_id", listId);

            using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        this.listItems.Add(this.listItemMapper.Convert(reader));
                    }
                    reader.Close();
                }

                databaseConnection.Close();
                command.Dispose();
            }
        }

        public List<ListItem> Items
        {
            get { return listItems; }
        }

        public long ListId { get; set; }

        private List<ListItem> listItems = new List<ListItem>();

        private ListItemMapper listItemMapper = new ListItemMapper();
    }
}
