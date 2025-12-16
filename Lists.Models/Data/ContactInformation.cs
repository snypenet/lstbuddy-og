using System;
using System.Collections.Generic;
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
    public class ContactInformation
    {
        private ContactInformationMapper contactInformationMapper = new ContactInformationMapper();
        public ContactInformation()
        {
        }

        public ContactInformation(long personId)
        {
            this.Load(personId);
        }

        public long PersonId { get; set; }
        public string Email { get; set; }
        public string CellPhoneNumber { get; set; }
        public CellPhoneProvider CellPhoneProvider { get; set; }

        public void Load(long personId)
        {
            using (var command = new SqlCommand(SQL.GetPersonContactInfoByPersonId))
            {
                command.Parameters.AddWithValue("@person_id", personId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.contactInformationMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            using (var command = new SqlCommand(SQL.SaveContactInformation))
            {
                command.Parameters.AddWithValue("@person_id", this.PersonId);
                command.Parameters.AddWithValue("@email", this.Email);

                if (this.CellPhoneProvider == null)
                {
                    command.Parameters.AddWithValue("@cell_phone_provider", 0);
                }
                else
                {
                    command.Parameters.AddWithValue("@cell_phone_provider", this.CellPhoneProvider.Id);
                }

                if (string.IsNullOrWhiteSpace(this.CellPhoneNumber))
                {
                    command.Parameters.AddWithValue("@cell_phone_number", "");
                }
                else
                {
                    command.Parameters.AddWithValue("@cell_phone_number", this.CellPhoneNumber);
                }

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
            using (var command = new SqlCommand(SQL.UpdateContactInformationByPersonId))
            {
                command.Parameters.AddWithValue("@email", this.Email);
                command.Parameters.AddWithValue("@person_id", this.PersonId);

                if (this.CellPhoneProvider == null)
                {
                    command.Parameters.AddWithValue("@cell_phone_provider", 0);
                }
                else
                {
                    command.Parameters.AddWithValue("@cell_phone_provider", this.CellPhoneProvider.Id);
                }

                if (string.IsNullOrWhiteSpace(this.CellPhoneNumber))
                {
                    command.Parameters.AddWithValue("@cell_phone_number", "");
                }
                else
                {
                    command.Parameters.AddWithValue("@cell_phone_number", this.CellPhoneNumber);
                }

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public void Copy(ContactInformation from)
        {
            this.PersonId = from.PersonId;
            this.Email = from.Email;
            this.CellPhoneProvider = from.CellPhoneProvider;
            this.CellPhoneNumber = from.CellPhoneNumber;
        }

        public static List<CellPhoneProvider> CellAllPhoneProviders()
        {
            var providers = new List<CellPhoneProvider>();

            using (var command = new SqlCommand(SQL.GetCellPhoneProviders))
            {
                var cellPhoneProviderMapper = new CellPhoneProviderMapper();
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            providers.Add(cellPhoneProviderMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            return providers;
        }
    }
}
