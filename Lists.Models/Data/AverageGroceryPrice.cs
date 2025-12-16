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
    public class AverageGroceryPrice
    {
        public string ItemName { get; set; }
        public decimal AverageCost { get; set; }
        
        public static void Delete(string itemName)
        {
            using (var command = new SqlCommand(SQL.DeleteAverageGroceryListItem))
            {
                command.Parameters.AddWithValue("@grocery_item_name", itemName);

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
            using (var command = new SqlCommand(SQL.SaveAverageGroceryListItem))
            {
                command.Parameters.AddWithValue("@grocery_item_name", this.ItemName);
                command.Parameters.AddWithValue("@average_price", this.AverageCost);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public static AverageGroceryPrice Get(string itemName)
        {
            AverageGroceryPrice item = null;
            using (var command = new SqlCommand(SQL.GetSaveAverageGroceryPrice))
            {
                command.Parameters.AddWithValue("@grocery_item_name", itemName);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            item = new AverageGroceryPrice
                            {
                                AverageCost = reader.GetDecimal(0),
                                ItemName = itemName
                            };
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
            return item;
        }

        public static void UpdateAverageGroceryPrices()
        {
            var items = new List<string>();
            using (var command = new SqlCommand(SQL.GetAllGroceryItems))
            {
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(reader.GetString(0).ToLower());
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            foreach (string item in items)
            {
                Delete(item);
                using (var command = new SqlCommand(string.Format(SQL.GetAverageGroceryPrice, item)))
                {
                    using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                    {
                        command.Connection = databaseConnection;
                        databaseConnection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                new AverageGroceryPrice
                                {
                                    AverageCost = reader.GetDecimal(0),
                                    ItemName = item
                                }.Save();
                            }
                            reader.Close();
                        }
                        databaseConnection.Close();
                    }
                }
            }
        }
    }
}
