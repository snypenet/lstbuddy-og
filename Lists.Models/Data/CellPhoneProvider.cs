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
    public class CellPhoneProvider
    {
        private CellPhoneProviderMapper cellPhoneProviderMapper = new CellPhoneProviderMapper();

        public CellPhoneProvider()
        {}

        public CellPhoneProvider(long id)
        {
            this.Load(id);
        }

        public void Load(long id)
        {
            using (var command = new SqlCommand(SQL.GetCellPhoneProviderById))
            {
                command.Parameters.AddWithValue("@provider_id", id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.cellPhoneProviderMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Copy(CellPhoneProvider from)
        {
            this.Name = from.Name;
            this.Id = from.Id;
            this.AddressFormat = from.AddressFormat;
        }

        public string Name { get; set; }
        public long Id { get; set; }
        public string AddressFormat { get; set; }
    }
}
