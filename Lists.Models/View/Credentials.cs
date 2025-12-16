using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Lists.Models.Data;
using System.Data.SqlClient;
using Lists.Constants;
using System.Configuration;
using Lists.Models.Mapping;

namespace Lists.Models.View
{
    public class Credentials
    {
        [Required(ErrorMessage = "Enter a valid pattern")]
        public string Pattern { get; set; }

        [Required(ErrorMessage="Enter a valid username")]
        public string Username { get; set; }

        public Person Authenticate()
        {
            var command = new SqlCommand(SQL.GetPersonByUsernameAndPattern);
            command.Parameters.AddWithValue("@username", this.Username);
            command.Parameters.AddWithValue("@pattern", this.Pattern);

            Person person = null;

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        person = this.personMapper.Convert(reader);
                    }
                    reader.Close();
                }

                databaseConnection.Close();
                command.Dispose();
            }

            return person;
        }

        public void Save()
        {
            using (var command = new SqlCommand(SQL.SaveCredentials))
            {
                command.Parameters.AddWithValue("@username", this.Username);
                command.Parameters.AddWithValue("@pattern", this.Pattern);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public void ReplacePattern()
        {
            using (var command = new SqlCommand(SQL.ReplacePattern))
            {
                command.Parameters.AddWithValue("@username", this.Username);
                command.Parameters.AddWithValue("@pattern", this.Pattern);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        private PersonMapper personMapper = new PersonMapper();
    }
}
