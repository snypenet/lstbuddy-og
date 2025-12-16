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
    public class QuickAddGroceryItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public long PersonId { get; set; }
        public bool UserEntered { get; set; }
        public bool IsHidden { get; set; }

        public static List<QuickAddGroceryItem> Get(long personId, bool userEntered = false)
        {
            var items = new List<QuickAddGroceryItem>();
            using (var command = new SqlCommand(SQL.GetQuickAddGroceryItems))
            {
                command.Parameters.AddWithValue("@personid", personId);
                command.Parameters.AddWithValue("@user_entered", userEntered);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new QuickAddGroceryItem
                            {
                                ItemName = reader.GetString(reader.GetOrdinal("quick_grocery_name")),
                                PersonId = personId,
                                UserEntered = userEntered,
                                Id = reader.GetInt32(reader.GetOrdinal("item_id")),
                                IsHidden = reader.GetBoolean(reader.GetOrdinal("is_hidden"))
                            });
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
            return items;
        }

        public QuickAddGroceryItem() { }

        public QuickAddGroceryItem(int id)
        {
            Load(id);
        }

        public void Load(int id)
        {
            using (var command = new SqlCommand(SQL.GetQuickItemById))
            {
                command.Parameters.AddWithValue("@item_id", id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Id = id;
                            ItemName = reader.GetString(reader.GetOrdinal("quick_grocery_name"));
                            PersonId = reader.GetInt64(reader.GetOrdinal("personid"));
                            UserEntered = reader.GetBoolean(reader.GetOrdinal("user_entered"));
                            IsHidden = reader.GetBoolean(reader.GetOrdinal("is_hidden"));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Update()
        {
            using (var command = new SqlCommand(SQL.UpdateQuickAddItem))
            {
                command.Parameters.AddWithValue("@item_id", Id);
                command.Parameters.AddWithValue("@is_hidden", IsHidden);

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
            using (var command = new SqlCommand(SQL.SaveQuickAddGroceryItem))
            {
                command.Parameters.AddWithValue("@quick_grocery_item", this.ItemName);
                command.Parameters.AddWithValue("@person_id", this.PersonId);
                command.Parameters.AddWithValue("@user_entered", UserEntered);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Id = reader.GetInt32(0);
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Delete()
        {
            using (var command = new SqlCommand(SQL.DeleteQuickAddItemById))
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

        public static void DeleteAll(long personId)
        {
            using (var command = new SqlCommand(SQL.DeleteQuickAddGroceryItems))
            {
                command.Parameters.AddWithValue("@person_id", personId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public static void UpdateQuickAddItems()
        {
            var personIds = new List<long>();
            using (var command = new SqlCommand(SQL.GetAllPersonIds))
            {
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            personIds.Add(reader.GetInt64(0));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            foreach (long personId in personIds)
            {
                var items = Get(personId);

                using (var command = new SqlCommand(SQL.GetTop10GroceryItems))
                {
                    command.Parameters.AddWithValue("@person_id", personId);
                    using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                    {
                        command.Connection = databaseConnection;
                        databaseConnection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(!items.Select(i => i.ItemName).Contains(reader.GetString(0)))
                                {
                                    new QuickAddGroceryItem
                                    {
                                        ItemName = reader.GetString(0),
                                        PersonId = personId
                                    }.Save();
                                }
                            }
                            reader.Close();
                        }
                        databaseConnection.Close();
                    }
                }

                var allItems = Get(personId);

                foreach (var quickItem in items.Where(a => !allItems.Select(i => i.ItemName).Contains(a.ItemName)))
                {
                    quickItem.Delete();
                }
            }
        }
    }
}
