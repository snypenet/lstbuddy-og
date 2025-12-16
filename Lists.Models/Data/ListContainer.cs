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
    public class ListContainer
    {
        public ListContainer(bool load = false)
        {
            if (load)
            {
                Load();
            }
        }

        public ListContainer Filter(Func<List, bool> predicate)
        {
            var collection = new ListContainer();
            collection.AddRange(lists.Where(predicate));
            return collection;
        }

        public void Load()
        {
            this.lists.Clear();
            using (var command = new SqlCommand(SQL.GetAllLists))
            {
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.lists.Add(this.listMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public static ListContainer GetPage(int fromRow, int toRow, int pageSize)
        {
            var listMapper = new ListMapper();

            string sql = string.Format(@"SELECT * 
                                         FROM (SELECT ROW_NUMBER() OVER (ORDER BY list_id DESC) As RowNum, *
                                               FROM lists_new
                                               WHERE deleted = 0) As RowConstrainedResult
                                         WHERE RowNum >= {0}
                                         AND RowNum < {1}
                                         ORDER BY RowNum", fromRow, toRow);

            var listItems = new List<List>();

            using (var command = new SqlCommand(sql))
            {
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listItems.Add(listMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            var collection = new ListContainer();
            collection.AddRange(listItems);

            if (listItems.Any())
            {
                sql = string.Format(@"SELECT * 
                                         FROM (SELECT ROW_NUMBER() OVER (ORDER BY list_id DESC) As RowNum, *
                                               FROM lists_new
                                               WHERE deleted = 0) As RowConstrainedResult
                                         WHERE RowNum < {0}
                                         ORDER BY RowNum", fromRow);

                using (var command = new SqlCommand(sql))
                {
                    using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                    {
                        command.Connection = databaseConnection;
                        databaseConnection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                collection.PreviousFrom = fromRow - pageSize;
                                collection.PreviousTo = fromRow;
                            }
                            reader.Close();
                        }
                        databaseConnection.Close();
                    }
                }


                sql = string.Format(@"SELECT * 
                                         FROM (SELECT ROW_NUMBER() OVER (ORDER BY list_id DESC) As RowNum, *
                                               FROM lists_new
                                               WHERE deleted = 0) As RowConstrainedResult
                                         WHERE RowNum >= {0}
                                         ORDER BY RowNum", toRow);

                using (var command = new SqlCommand(sql))
                {
                    using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                    {
                        command.Connection = databaseConnection;
                        databaseConnection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                collection.NextFrom = toRow;
                                collection.NextTo = toRow + pageSize;
                            }
                            reader.Close();
                        }
                        databaseConnection.Close();
                    }
                }
            }

            return collection;
        }

        public static ListContainer SearchLists(string query)
        {
            query = query.Trim();

            var lists = new ListContainer();


            if (!string.IsNullOrWhiteSpace(query))
            {
                var sql = new StringBuilder();
                sql.Append(@"SELECT TOP 100 l.* 
                         FROM lists_new l (NOLOCK)
                         JOIN list_types lt (NOLOCK)
                         ON lt.list_type_id = l.list_type
                         JOIN persons p (NOLOCK) 
                         ON p.person_id = l.owner_id 
                         WHERE (1=1) ");

                var queryParts = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (queryParts.Any())
                {
                    sql.AppendFormat("AND (l.list_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR p.f_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR p.m_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR p.l_name LIKE '%{0}%' ", query);
                    foreach (string part in queryParts)
                    {
                        sql.AppendFormat("OR l.list_name LIKE '%{0}%' ", part);
                        sql.AppendFormat("OR lt.list_type LIKE '%{0}%' ", part);
                        sql.AppendFormat("OR p.f_name LIKE '%{0}%' ", part);
                        sql.AppendFormat("OR p.l_name LIKE '%{0}%' ", part);
                        sql.AppendFormat("OR p.m_name LIKE '%{0}%' ", part);
                    }
                    sql.Append(") ");
                }
                else
                {
                    sql.AppendFormat("AND (l.list_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR p.f_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR p.m_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR p.l_name LIKE '%{0}%') ", query);
                }

                sql.Append("AND l.deleted = 0 ");
                sql.Append("ORDER BY l.created_on DESC ");

                using (var command = new SqlCommand(sql.ToString()))
                {
                    using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                    {
                        command.Connection = connection;
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            var mapper = new ListMapper();

                            while (reader.Read())
                            {
                                lists.Add(mapper.Convert(reader));
                            }
                            reader.Close();
                        }
                        connection.Close();
                    }
                }
                lists.OrderBy(false, l => l.IsExpired);
            }
            else
            {
                //if no query is provided then default to normal paged results
                lists = ListContainer.GetPage(0, 10, 10);
            }

            return lists;
        }

        public void OrderBy<Tkey>(bool descending, Func<List, Tkey> func)
        {
            lists = descending ? lists.OrderByDescending(func).ToList() : lists.OrderBy(func).ToList();
        }

        private List<List> lists = new List<List>();
        private ListMapper listMapper = new ListMapper();
        public int PreviousFrom { get; set; }
        public int PreviousTo { get; set; }
        public int NextFrom { get; set; }
        public int NextTo { get; set; }

        public List<List> Lists
        {
            get { return lists; }
        }

        public void Add(List list)
        {
            lists.Add(list);
        }

        public void AddRange(IEnumerable<List> l)
        {
            lists.AddRange(l);
        }
    }
}
