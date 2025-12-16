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
    public class EditorContainer
    {
        public EditorContainer()
        {
        }

        public long ListId { get; set; }

        public EditorContainer(long listId)
        {
            this.Load(listId);
        }

        public void Load(long listId)
        {
            ListId = listId;
            this.people.Clear();
            using (var command = new SqlCommand(SQL.GetListEditorsByListId))
            {
                command.Parameters.AddWithValue("@list_id", listId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
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
                }
            }
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
            using (var command = new SqlCommand(SQL.DeleteEditorByListIdPersonId))
            {
                command.Parameters.AddWithValue("@list_id", this.ListId);
                command.Parameters.AddWithValue("@editor_id", personId);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public void Save(long personId)
        {
            using (var command = new SqlCommand(SQL.SaveEditor))
            {
                command.Parameters.AddWithValue("@list_id", this.ListId);
                command.Parameters.AddWithValue("@editor_id", personId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public bool Contains(long personId)
        {
            return people.Any(person => person.Id == personId);
        }

        public List<Person> Editors
        {
            get { return people; }
        }

        private PersonMapper personMapper = new PersonMapper();
        private List<Person> people = new List<Person>();
    }
}
