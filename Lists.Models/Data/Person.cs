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
using Lists.Models.Utils;
using System.Security.Policy;

namespace Lists.Models.Data
{
    public class Person
    {
        private PersonMapper personMapper = new PersonMapper();

        public Person()
        {
        }

        public Person(long personid)
        {
            this.Load(personid);
        }

        public static bool DoesUsernameExist(string username)
        {
            var command = new SqlCommand(SQL.DoesUsernameExist);
            command.Parameters.AddWithValue("@username", username);

            bool exists = false;

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    exists = reader.GetInt32(0) > 0;
                    reader.Close();
                }

                databaseConnection.Close();
                command.Dispose();
            }

            return exists;
        }

        public static Person GetPersonByEmailAndUsername(string email, string username)
        {
            var command = new SqlCommand(SQL.GetPersonIdByEmailAndUsername);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@email", email);

            Person result = null;

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        result = new Person(reader.GetInt64(0)); 
                    }
                }

                databaseConnection.Close();
                command.Dispose();
            }

            return result;
        }

        public void Load(long personid)
        {
            using (var command = new SqlCommand(SQL.GetPersonById))
            {
                command.Parameters.AddWithValue("@person_id", personid);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.personMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            using (var command = new SqlCommand(SQL.SavePerson))
            {
                command.Parameters.AddWithValue("@f_name", this.FirstName);

                if (string.IsNullOrWhiteSpace(this.MiddleName))
                {
                    command.Parameters.AddWithValue("@m_name", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@m_name", this.MiddleName);
                }

                command.Parameters.AddWithValue("@l_name", this.LastName);
                command.Parameters.AddWithValue("@username", this.Username);
                command.Parameters.AddWithValue("@is_guest", this.IsGuest);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        this.Id = reader.GetInt64(0);
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Update()
        {
            using (var command = new SqlCommand(SQL.UpdatePersonById))
            {
                command.Parameters.AddWithValue("@f_name", this.FirstName);

                if (string.IsNullOrWhiteSpace(this.MiddleName))
                {
                    command.Parameters.AddWithValue("@m_name", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@m_name", this.MiddleName);
                }

                command.Parameters.AddWithValue("@l_name", this.LastName);
                command.Parameters.AddWithValue("@username", this.Username);
                command.Parameters.AddWithValue("@person_id", this.Id);
                command.Parameters.AddWithValue("@is_guest", this.IsGuest);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        #region Send Emails
        public void SendForgotPatternEmail()
        {
            var changeControl = new ChangeControl();
            changeControl.Generate();
            string href = string.Format("{0}/Profile/ResetPattern/{1}",ConfigurationManager.AppSettings["RootUrl"], changeControl.Id);
            QueuedEmail.Add(ContactInfo.Email,
                string.Format("To reset your pattern for the Lst Buddy website please go to this <a href='{0}'>link</a>", href),
                "Lst Buddy - Reset Pattern");
        }

        public void SendCommentResponseEmail(string listName)
        {
            string href = string.Format("{0}/Logon", ConfigurationManager.AppSettings["RootUrl"]);
            QueuedEmail.Add(ContactInfo.Email,
                string.Format("Someone has commented on an item you have commented on in '{0}'. Logon <a href='{1}'>here</a> to see the comment", listName, href),
                "Lst Buddy - Comment Response");
        }

        public void SendSomeoneCommentedEmail(string listName)
        {
            string href = string.Format("{0}/Logon", ConfigurationManager.AppSettings["RootUrl"]);
            QueuedEmail.Add(ContactInfo.Email,
                string.Format("Someone has commented on an item in your list, '{0}'. Logon <a href='{1}'>here</a> to see the comment", listName, href),
                "Lst Buddy - Someone Commented");
        }

        public void SendListSecurityChangeEmail(string listName)
        {
            string href = string.Format("{0}/Logon", ConfigurationManager.AppSettings["RootUrl"]);
            QueuedEmail.Add(ContactInfo.Email,
                string.Format("Your security on list, '{0}', has changed.  Logon <a href='{1}'>here</a> to see the list", listName, href),
                "Lst Buddy - List Security Changed");
        }

        public void SendNewListItemEmail(string listName)
        {
            string href = string.Format("{0}/Logon", ConfigurationManager.AppSettings["RootUrl"]);
            QueuedEmail.Add(ContactInfo.Email,
                string.Format("A new list item has been added to, '{0}'.  Logon <a href='{1}'>here</a> to see the list item", listName, href),
                "Lst Buddy - New List Item");
        }
        #endregion

        public void Delete()
        {
            using (var command = new SqlCommand(SQL.DeletePersonById))
            {
                command.Parameters.AddWithValue("@person_id", this.Id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public static List<PersonSearchResult> LookupByName(string query)
        {
            query = query.Trim();

            var results = new List<PersonSearchResult>();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var sql = new StringBuilder();
                sql.AppendLine(@"SELECT distinct person_id, f_name, l_name 
                                 FROM persons 
                                 WHERE(1=1) ");

                var queryParts = query.Split(new char[] { ' ' });
                if (queryParts.Any())
                {
                    sql.AppendFormat("AND (f_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR m_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR l_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR username LIKE '%{0}%' ", query);

                    foreach (string part in queryParts)
                    {
                        sql.AppendFormat("OR f_name LIKE '%{0}%' ", part);
                        sql.AppendFormat("OR m_name LIKE '%{0}%' ", part);
                        sql.AppendFormat("OR l_name LIKE '%{0}%' ", part);
                        sql.AppendFormat("OR username LIKE '%{0}%' ", part);
                    }

                    sql.Append(") ");
                }
                else
                {
                    sql.AppendFormat("AND (f_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR m_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR l_name LIKE '%{0}%' ", query);
                    sql.AppendFormat("OR username LIKE '%{0}%') ", query);
                }

                sql.AppendLine("AND is_guest = 0 AND deleted = 0 ");

                using (var command = new SqlCommand(sql.ToString()))
                {
                    command.Parameters.AddWithValue("@query", query);
                    using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                    {
                        command.Connection = databaseConnection;
                        databaseConnection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new PersonSearchResult
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("f_name")),
                                    Id = reader.GetInt64(reader.GetOrdinal("person_id")),
                                    LastName = reader.GetString(reader.GetOrdinal("l_name"))
                                });
                            }
                            reader.Close();
                        }
                        databaseConnection.Close();
                    }
                }
            }

            return results;
        }

        public void Copy(Person from)
        {
            this.FirstName = from.FirstName;
            this.Id = from.Id;
            this.LastName = from.LastName;
            this.Username = from.Username;
            this.MiddleName = from.MiddleName;
            this.ContactInfo = from.ContactInfo;
            this.IsGuest = from.IsGuest;
            this.IsAdmin = from.IsAdmin;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long Id { get; set; }
        public string Username { get; set; }
        public ContactInformation ContactInfo { get; set; }
        public bool IsGuest { get; set; }
        public bool IsAdmin { get; set; }
    }
}
