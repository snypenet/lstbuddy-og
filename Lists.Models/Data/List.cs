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
    public class List 
    {
        public static void DeleteExpiredGuestLists()
        {
            using (var command = new SqlCommand(SQL.DeleteExpiredGuestLists))
            {
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public static void Undelete(long id)
        {
            using (var command = new SqlCommand(SQL.UndeleteListById))
            {
                command.Parameters.AddWithValue("@list_id", id);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public List()
        {
        }

        public List(long listId)
        {
            this.Load(listId);
        }

        public List(string shareId)
        {
            this.Load(shareId);
        }

        public void AutoPopulate()
        {
            using (var command = new SqlCommand(SQL.AutoPopulateGroceryList))
            {
                command.Parameters.AddWithValue("@list_id", Id);
                command.Parameters.AddWithValue("@created_on", CreatedOn);
                command.Parameters.AddWithValue("@created_by", CreatedBy.Id);
                command.Parameters.AddWithValue("@last_modified_on", CreatedOn);
                command.Parameters.AddWithValue("@last_modified_by", CreatedBy.Id);
                command.Parameters.AddWithValue("@owner_id", Owner.Id);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public string Name { get; set; }
        public ListType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public Person Owner { get; set; }
        public Person CreatedBy { get; set; }
        public long Id { get; set; }
        public bool IsPublic { get; set; }
        public string ShareId { get; set; }
        public bool IsPrivate { get; set; }

        private ListItemContainer items;
        public ListItemContainer Items 
        {
            get
            {
                if (items == null)
                {
                    items = new ListItemContainer(Id);
                }

                return items;
            }
            set
            {
                items = value;
            }
        }

        private EditorContainer editors;
        public EditorContainer Editors 
        {
            get
            {
                if (editors == null)
                {
                    editors = new EditorContainer(Id);
                }
                return editors;
            }
            set
            {
                editors = value;
            }
        }

        private ViewerContainer viewers;
        public ViewerContainer Viewers 
        {
            get
            {
                if (viewers == null)
                {
                    viewers = new ViewerContainer(Id);
                }
                return viewers;
            }
            set
            {
                viewers = value;
            }
        }

        public bool IsExpired
        {
            get
            {
                return this.ExpiresOn.HasValue && this.ExpiresOn.Value < DateTime.Now;
            }
        }

        public void Load(long listId)
        {
            using (var command = new SqlCommand(SQL.GetListById))
            {
                command.Parameters.AddWithValue("@list_id", listId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.listMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Load(string shareId)
        {
            using (var command = new SqlCommand(SQL.GetListByShareId))
            {
                command.Parameters.AddWithValue("@share_id", shareId);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.listMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            var command = new SqlCommand(SQL.SaveList);
            command.Parameters.AddWithValue("@list_name", this.Name);
            command.Parameters.AddWithValue("@list_type", this.Type.Id);
            command.Parameters.AddWithValue("@created_on", this.CreatedOn);
            command.Parameters.AddWithValue("@created_by", this.CreatedBy.Id);
            command.Parameters.AddWithValue("@owner_id", this.Owner.Id);
            command.Parameters.AddWithValue("@is_public", this.IsPublic);
            command.Parameters.AddWithValue("@is_private", this.IsPrivate);

            if (this.ExpiresOn.HasValue)
            {
                command.Parameters.AddWithValue("@expires_on", this.ExpiresOn);
            }
            else
            {
                command.Parameters.AddWithValue("@expires_on", DBNull.Value);
            }

            if (string.IsNullOrWhiteSpace(ShareId))
            {
                ShareId = Guid.NewGuid().ToString();
            }

            command.Parameters.AddWithValue("@share_id", ShareId);
            
            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
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
                command.Dispose();
            }
        }

        public void Update()
        {
            var command = new SqlCommand(SQL.UpdateListById);
            command.Parameters.AddWithValue("@list_id", this.Id);
            command.Parameters.AddWithValue("@list_name", this.Name);
            command.Parameters.AddWithValue("@list_type", this.Type.Id);
            command.Parameters.AddWithValue("@owner_id", this.Owner.Id);
            command.Parameters.AddWithValue("@is_public", this.IsPublic);
            command.Parameters.AddWithValue("@is_private", this.IsPrivate);

            if (this.ExpiresOn.HasValue)
            {
                command.Parameters.AddWithValue("@expires_on", this.ExpiresOn);
            }
            else
            {
                command.Parameters.AddWithValue("@expires_on", DBNull.Value);
            }

            if (string.IsNullOrWhiteSpace(ShareId))
            {
                ShareId = Guid.NewGuid().ToString();
            }

            command.Parameters.AddWithValue("@share_id", ShareId);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                command.ExecuteNonQuery();

                databaseConnection.Close();
                command.Dispose();
            }
        }

        public void Delete()
        {
            var command = new SqlCommand(SQL.DeleteListById);
            command.Parameters.AddWithValue("@list_id", this.Id);

            using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
            {
                command.Connection = databaseConnection;
                databaseConnection.Open();

                command.ExecuteNonQuery();

                databaseConnection.Close();
                command.Dispose();
            }
        }

        public void Copy(List from)
        {
            this.CreatedBy = from.CreatedBy;
            this.CreatedOn = from.CreatedOn;
            this.ExpiresOn = from.ExpiresOn;
            this.Id = from.Id;
            this.Name = from.Name;
            this.Owner = from.Owner;
            this.Type = from.Type;
            this.Editors = from.Editors;
            this.Items = from.Items;
            this.Viewers = from.Viewers;
            this.IsPublic = from.IsPublic;
            this.ShareId = from.ShareId;
            this.IsPrivate = from.IsPrivate;
        }

        private ListMapper listMapper = new ListMapper();
    }
}
