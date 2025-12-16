using Lists.Constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Data
{
    public class AmazonItem
    {
        public static List<AmazonItem> GetItems(long itemId)
        {
            var items = new List<AmazonItem>();

            using (var command = new SqlCommand(@"SELECT distinct a.* 
                                                  FROM amazon_items a 
                                                  JOIN amazon_list_items i 
                                                  ON i.item_id = a.item_id
                                                  AND list_item_id = @list_item_id"))
            {
                command.Parameters.AddWithValue("@list_item_id", itemId);
                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new AmazonItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("item_id")),
                                Link = reader.GetString(reader.GetOrdinal("item_link")),
                                Name = reader.GetString(reader.GetOrdinal("item_name"))
                            });
                        }
                        reader.Close();
                    }

                    databaseConnection.Close();
                }
            }

            return items.GroupBy(i => i.Name).Select(g => g.First()).ToList();
        }

        public void MapTo(long listItemId)
        {
            using (var command = new SqlCommand("INSERT INTO amazon_list_items VALUES (@list_item_id, @item_id)"))
            {
                command.Parameters.AddWithValue("@item_id", Id);
                command.Parameters.AddWithValue("@list_item_id", listItemId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public static AmazonItem GetItem(string name)
        {
            AmazonItem item = null;

            using (var command = new SqlCommand(SQL.GetAmazonItemByName))
            {
                command.Parameters.AddWithValue("@item_name", name);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item = new AmazonItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("item_id")),
                                Link = reader.GetString(reader.GetOrdinal("item_link")),
                                Name = reader.GetString(reader.GetOrdinal("item_name"))
                            };
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            return item;
        }

        public void Delete()
        {
            using (var command = new SqlCommand(SQL.DeleteAmazonItemById))
            {
                command.Parameters.AddWithValue("@item_id", Id);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public void UnMapFrom(long listItemId)
        {
            using (var command = new SqlCommand("DELETE FROM amazon_list_items WHERE item_id = @item_id AND list_item_id = @list_item_id"))
            {
                command.Parameters.AddWithValue("@item_id", Id);
                command.Parameters.AddWithValue("@list_item_id", listItemId);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public void Update()
        {
            using (var command = new SqlCommand(SQL.UpdateAmazonItemById))
            {
                command.Parameters.AddWithValue("@item_id", Id);
                command.Parameters.AddWithValue("@item_link", Link);
                command.Parameters.AddWithValue("@item_name", Name);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            using (var command = new SqlCommand(@"INSERT INTO amazon_items (item_link, item_name) VALUES (@item_link, @item_name);
                                                  SELECT item_id FROM amazon_items WHERE item_id = SCOPE_IDENTITY()"))
            {
                command.Parameters.AddWithValue("@item_link", Link);
                command.Parameters.AddWithValue("@item_name", Name);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Id = reader.GetInt32(0);
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
