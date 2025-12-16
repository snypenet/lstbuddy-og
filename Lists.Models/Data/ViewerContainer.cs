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
    public class ViewerContainer
    {
        public ViewerContainer()
        {
        }

        public ViewerContainer(long listId)
        {
            this.Load(listId);
        }

        public long ListId { get; set; }
        public void Clear()
        {
            people.Clear();
        }

        public void Add(long personId)
        {
            var person = new Person(personId);
            people.Add(person);
            Save(personId);
            var list = new List(ListId);
            person.SendListSecurityChangeEmail(list.Name);
        }

        public void Remove(long personId)
        {
            if (Contains(personId))
            {
                people.RemoveAt(people.Select(person => person.Id).ToList().IndexOf(personId));
                Delete(personId);
            }
        }

        public void Delete(long personId)
        {
            var command = new SqlCommand(SQL.DeleteViewerByListIdPersonId);
            command.Parameters.AddWithValue("@list_id", this.ListId);
            command.Parameters.AddWithValue("@person_id", personId);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                command.ExecuteNonQuery();

                databaseConnection.Close();
                command.Dispose();
            }
        }

        public void Save(long personId)
        {
            var command = new SqlCommand(SQL.SaveViewer);
            command.Parameters.AddWithValue("@list_id", this.ListId);
            command.Parameters.AddWithValue("@person_id", personId);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
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
            return people.Any(person => person.Id == personId);
        }

        public void Load(long listId)
        {
            ListId = listId;
            this.people.Clear();
            var command = new SqlCommand(SQL.GetListViewersByListId);
            command.Parameters.AddWithValue("@list_id", listId);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        this.people.Add(new Person(reader.GetInt64(0)));
                    }
                    reader.Close();
                }

                databaseConnection.Close();
                command.Dispose();
            }
        }

        public List<Person> Viewers
        {
            get { return people; }
        }

        private List<Person> people = new List<Person>();

        private PersonMapper personMapper = new PersonMapper();
    }
}
