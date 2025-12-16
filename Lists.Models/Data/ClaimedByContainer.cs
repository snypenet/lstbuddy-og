using System;
using System.Collections.Generic;
using System.Collections;
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
    public class ClaimedByContainer
    {
        public ClaimedByContainer()
        {
        }

        public ClaimedByContainer(long listItemId)
        {
            this.Load(listItemId);
        }

        public long ListItemId { get; set; }

        public void Add(Person person)
        {
            claimedBy.Add(person);
            Save(person.Id);
        }

        public void Remove(Person person)
        {
            int index = claimedBy.Select(claimer => claimer.Id).ToList().IndexOf(person.Id);

            if (index >= 0)
            {
                claimedBy.RemoveAt(index);
                Delete(person.Id);
            }
        }

        private void Delete(long personId)
        {
            var command = new SqlCommand(SQL.DeleteClaimer);
            command.Parameters.AddWithValue("@person_id", personId);
            command.Parameters.AddWithValue("@list_item_id", ListItemId);

            using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                command.ExecuteNonQuery();

                databaseConnection.Close();
                command.Dispose();
            }
        }

        public bool Contains(long personId)
        {
            return claimedBy.Select(person => person.Id).Contains(personId);
        }

        public List<Person> People
        {
            get { return claimedBy; }
        }

        public void Load(long listItemId)
        {
            ListItemId = listItemId;
            this.claimedBy.Clear();
            using (var command = new SqlCommand(SQL.GetListItemClaimersByListItemId))
            {
                command.Parameters.AddWithValue("@list_item_id", listItemId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.claimedBy.Add(new Person(reader.GetInt64(0)));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save(long personid)
        {
            using (var command = new SqlCommand(SQL.SaveClaimer))
            {
                command.Parameters.AddWithValue("@person_id", personid);
                command.Parameters.AddWithValue("@list_item_id", ListItemId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        private List<Person> claimedBy = new List<Person>();
        private PersonMapper personMapper = new PersonMapper();
    }
}
