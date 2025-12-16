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
    public class ListAccessRequestContainer
    {
        private List<ListAccessRequest> requests = new List<ListAccessRequest>();

        public ListAccessRequestContainer()
        {
        }

        public bool Contains(long personId)
        {
            return requests.Select(r => r.Requestor.Id).Contains(personId);
        }

        private ListAccessRequestMapper listAccessRequestMapper = new ListAccessRequestMapper();

        public long ListId { get; set; }

        public ListAccessRequestContainer(long listId)
        {
            Load(listId);
        }

        public void Remove(int id)
        {
            int i = requests.Select(item => item.Id).ToList().IndexOf(id);
            if (i != -1)
            {
                requests.RemoveAt(i);
                Delete(id);
            }
        }

        public void Delete(int id)
        {
            var command = new SqlCommand(SQL.DeleteListAccessRequest);
            command.Parameters.AddWithValue("@id", id);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                command.ExecuteNonQuery();
                databaseConnection.Close();
            }
        }

        public List<ListAccessRequest> Requests
        {
            get { return requests; }
        }

        public void Load(long listId)
        {
            ListId = listId;
            var command = new SqlCommand(SQL.GetListAccessRequestByListId);
            command.Parameters.AddWithValue("@list_id", listId);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requests.Add(listAccessRequestMapper.Convert(reader));
                    }
                    reader.Close();
                }
                databaseConnection.Close();
            }
        }

        public void Add(IEnumerable<ListAccessRequest> items)
        {
            requests.AddRange(items);
        }

        public ListAccessRequestContainer Filter(Func<ListAccessRequest, bool> predicate)
        {
            var filtered = new ListAccessRequestContainer();
            filtered.Add(requests.Where(predicate));
            filtered.ListId = ListId;
            return filtered;
        }
    }
}
