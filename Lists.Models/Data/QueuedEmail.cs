using Lists.Constants;
using Lists.Models.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Data
{
    public class QueuedEmail 
    {
        private List<Email> emails = new List<Email>();

        public void Load()
        {
            using (var command = new SqlCommand(SQL.GetQueuedEmails))
            {
                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var email = new Email(reader.GetString(reader.GetOrdinal("to_email")), reader.GetString(reader.GetOrdinal("body")), reader.GetString(reader.GetOrdinal("subject")));
                            email.Id = reader.GetInt32(reader.GetOrdinal("id"));
                            emails.Add(email);
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Send()
        {
            if (emails.Any())
            {
                foreach (var email in emails)
                {
                    if (email.Send())
                    {
                        MarkAsSent(email.Id);
                    }
                }

                emails.Clear();
            }
        }

        public static void ProcessQueuedEmails()
        {
            var queuedEmail = new QueuedEmail();
            queuedEmail.Load();
            queuedEmail.Send();
        }

        public static void Add(string to, string body, string subject)
        {
            if (to != null)
            {
                using (var command = new SqlCommand(SQL.AddToEmailQueue))
                {
                    command.Parameters.AddWithValue("@to_email", to);
                    command.Parameters.AddWithValue("@body", body);
                    command.Parameters.AddWithValue("@subject", subject);
                    command.Parameters.AddWithValue("@createdOn", DateTime.Now);

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

        public void MarkAsSent(int emailId)
        {
            using (var command = new SqlCommand(SQL.MarkEmailAsSent))
            {
                command.Parameters.AddWithValue("@id", emailId);

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
}
