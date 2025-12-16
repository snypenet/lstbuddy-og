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
    public class AttachmentData
    {
        public AttachmentData(long attachmentDataId)
        {
            this.attachmentDataMapper = new AttachmentDataMapper();
            this.Id = attachmentDataId;
        }

        public AttachmentData()
        {
            this.attachmentDataMapper = new AttachmentDataMapper();
        }

        public void Load(long attachmentDataId)
        {
            using (var command = new SqlCommand(SQL.GetAttachmentDataById))
            {
                command.Parameters.AddWithValue("@attachment_data_id", attachmentDataId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.attachmentDataMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }


        public void Save()
        {
            using (var command = new SqlCommand(SQL.SaveAttachmentData))
            {
                command.Parameters.AddWithValue("@attachment_data", this.Data);
                command.Parameters.AddWithValue("@attachment_data_mime", this.Mime);

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
            using (var command = new SqlCommand(SQL.UpdateAttachmentDataById))
            {
                command.Parameters.AddWithValue("@attachment_data_id", this.Id);
                command.Parameters.AddWithValue("@attachment_data", this.Data);
                command.Parameters.AddWithValue("@attachment_data_mime", this.Mime);

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
            using (var command = new SqlCommand(SQL.DeleteAttachmentDataById))
            {
                command.Parameters.AddWithValue("@attachment_data_id", this.Id);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public long Id { get; set; }
        public byte[] Data 
        {
            get
            {
                if (data == null)
                {
                    this.Load(this.Id);
                }

                return data;
            }
            set
            {
                data = value;
            }
        }
        public string Mime { get; set; }

        public void Copy(AttachmentData from)
        {
            this.Data = from.Data;
            this.Id = from.Id;
            this.Mime = from.Mime;
        }

        private AttachmentDataMapper attachmentDataMapper;
        private byte[] data;
    }
}
