using Lists.Constants;
using Lists.Models.Mapping;
using Lists.Models.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Data
{
    public class ListAccessRequest
    {
        public int Id { get; set; }
        public Person Requestor { get; set; }
        public List List { get; set; }
        public ListSecurityType AccessType { get; set; }

        private ListAccessRequestMapper listAccessRequestMapper = new ListAccessRequestMapper();

        public ListAccessRequest()
        {
        }

        public ListAccessRequest(int id)
        {
            Load(id);
        }

        public void Save()
        {
            var command = new SqlCommand(SQL.SaveListAccessRequest);
            command.Parameters.AddWithValue("@person_id", this.Requestor.Id);
            command.Parameters.AddWithValue("@list_id", this.List.Id);
            command.Parameters.AddWithValue("@access_type", (int)this.AccessType);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
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

        public void Load(int id)
        {
            var command = new SqlCommand(SQL.GetListAccessRequestById);
            command.Parameters.AddWithValue("@id", id);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Copy(this.listAccessRequestMapper.Convert(reader));
                    }
                    reader.Close();
                }
                databaseConnection.Close();
            }
        }

        public void Delete()
        {
            var command = new SqlCommand(SQL.DeleteListAccessRequest);
            command.Parameters.AddWithValue("@id", this.Id);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                command.ExecuteNonQuery();
                databaseConnection.Close();
            }
        }

        public void Copy(ListAccessRequest from)
        {
            Id = from.Id;
            Requestor = from.Requestor;
            List = from.List;
            AccessType = from.AccessType;
        }
    }
}
