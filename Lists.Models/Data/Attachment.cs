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
    public class Attachment
    {
        public Attachment()
        {
            this.attachmentMapper = new AttachmentMapper();
        }

        public Attachment(long attachmentId)
        {
            this.attachmentMapper = new AttachmentMapper();
            this.Load(attachmentId);
        }

        public void Load(long attachmentId)
        {
            using (var command = new SqlCommand(SQL.GetAttachmentById))
            {
                command.Parameters.AddWithValue("@attachment_id", attachmentId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.attachmentMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            using (var command = new SqlCommand(SQL.SaveAttachment))
            {
                command.Parameters.AddWithValue("@attachment_type", this.AttachmentType);
                command.Parameters.AddWithValue("@attachment_description", this.Description);
                command.Parameters.AddWithValue("@attachment_data_id", this.Data.Id);
                command.Parameters.AddWithValue("@list_item_id", this.ListItemId);

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
            using (var command = new SqlCommand(SQL.UpdateAttachmentById))
            {
                command.Parameters.AddWithValue("@attachment_data_id", this.Data.Id);
                command.Parameters.AddWithValue("@attachment_type", this.AttachmentType);
                command.Parameters.AddWithValue("@attachment_description", this.Description);
                command.Parameters.AddWithValue("@list_item_id", this.ListItemId);
                command.Parameters.AddWithValue("@attachment_id", this.Id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public void Delete()
        {
            using (var command = new SqlCommand(SQL.DeleteAttachmentById))
            {
                command.Parameters.AddWithValue("@attachment_id", this.Id);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public long Id { get; set; }
        public long AttachmentType { get; set; }
        public string Description { get; set; }
        public AttachmentData Data { get; set; }
        public long ListItemId { get; set; }

        public void Copy(Attachment from)
        {
            this.Id = from.Id;
            this.Description = from.Description;
            this.Data = from.Data;
            this.ListItemId = this.ListItemId;
            this.AttachmentType = from.AttachmentType;
        }

        private AttachmentMapper attachmentMapper;
    }
}
