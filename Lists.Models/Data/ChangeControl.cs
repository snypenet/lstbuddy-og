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
    public class ChangeControl
    {
        private ChangeControlMapper mapper = new ChangeControlMapper();

        public string Id { get; set; }
        public bool Used { get; set; }
        public bool IsValid { get; set; }

        public ChangeControl()
        {}

        public ChangeControl(string id)
        {
            this.Load(id);
        }

        public void Load(string id)
        {
            using (var command = new SqlCommand(SQL.GetChangeControlById))
            {
                command.Parameters.AddWithValue("@change_id", id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.IsValid = true;
                            this.Copy(this.mapper.Convert(reader));
                        }
                        else
                        {
                            this.IsValid = false;
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Generate()
        {
            this.Id = Guid.NewGuid().ToString();
            using (var command = new SqlCommand(SQL.SaveChangeControl))
            {
                command.Parameters.AddWithValue("@change_id", this.Id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.mapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void MarkUsed()
        {
            using (var command = new SqlCommand(SQL.MarkChangeControlUsed))
            {
                command.Parameters.AddWithValue("@change_id", this.Id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.mapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Copy(ChangeControl from)
        {
            this.Id = from.Id;
            this.Used = from.Used;
        }
    }
}
