using Lists.Constants;
using Lists.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Data
{
    public class SiteMessage
    {
        private static SiteMessageMapper staticSiteMessageMapper = new SiteMessageMapper();
        private static SiteMessageMapper siteMessageMapper = new SiteMessageMapper();
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime ShowFrom { get; set; }
        public DateTime ShowTo { get; set; }
        public SiteMessageType Type { get; set; }

        public static List<SiteMessage> GetAllActive()
        {
            var messages = new List<SiteMessage>();

            var command = new SqlCommand(SQL.GetActiveSiteMessages);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        messages.Add(staticSiteMessageMapper.Convert(reader));
                    }
                    reader.Close();
                }

                databaseConnection.Close();
                command.Dispose();
            }

            return messages;
        }

        public void Save()
        {
            var command = new SqlCommand(SQL.InsertSiteMessage);
            command.Parameters.AddWithValue("@text", this.Text);
            command.Parameters.AddWithValue("@show_from", this.ShowFrom);
            command.Parameters.AddWithValue("@show_to", this.ShowTo);
            command.Parameters.AddWithValue("@type", (int)this.Type);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                command.ExecuteNonQuery();

                databaseConnection.Close();
                command.Dispose();
            }
        }
    }
}
