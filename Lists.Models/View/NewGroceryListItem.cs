using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Lists.Models.Data;
using System.Data.SqlClient;
using Lists.Constants;
using System.Configuration;

namespace Lists.Models.View
{
    public class NewGroceryListItem : NewListItem
    {
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        
        public int? Quantity { get; set; }

        public IEnumerable<string> QuickAdd { get; set; }

        public void GuessPrice()
        {
            using (var command = new SqlCommand(SQL.GetSaveAverageGroceryPrice))
            {
                command.Parameters.AddWithValue("@grocery_item_name", Text);
                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            Price = reader.GetDecimal(0);
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }
    }
}
