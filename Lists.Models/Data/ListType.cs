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
    public class ListType
    {
        private ListTypeMapper listTypeMapper = new ListTypeMapper();

        public ListType()
        { }

        public ListType(long id)
        {
            this.Load(id);
        }

        public void Load(long id)
        {
            var command = new SqlCommand(SQL.GetListTypeById);
            command.Parameters.AddWithValue("@list_type_id", id);

            using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        this.Copy(this.listTypeMapper.Convert(reader));
                    }
                    reader.Close();
                }

                databaseConnection.Close();
                command.Dispose();
            }
        }

        public static List<ListType> AllListTypes()
        {
            var command = new SqlCommand(SQL.GetAllListTypes);
            var listTypes = new List<ListType>();
            var listTypeMapper = new ListTypeMapper();

            using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listTypes.Add(listTypeMapper.Convert(reader));
                    }
                    reader.Close();
                }

                databaseConnection.Close();
                command.Dispose();
            }

            return listTypes;
        }

        public void Copy(ListType from)
        {
            this.Name = from.Name;
            this.Id = from.Id;
        }

        public string Name { get; set; }
        public long Id { get; set; }
    }
}
