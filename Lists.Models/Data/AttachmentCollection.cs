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
    public class AttachmentCollection : 
        IEnumerable<Attachment>
    {
        public AttachmentCollection()
        {
            this.attachments = new List<Attachment>();
            this.attachmentMapper = new AttachmentMapper();
        }

        public AttachmentCollection(long listItemId)
        {
            this.attachments = new List<Attachment>();
            this.attachmentMapper = new AttachmentMapper();
            this.Load(listItemId);
        }

        public void Load(long listItemId)
        {
            this.attachments.Clear();
            using (var command = new SqlCommand(SQL.GetAttachmentsByListItemId))
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
                            this.attachments.Add(this.attachmentMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public IEnumerator<Attachment> GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private List<Attachment> attachments;

        private AttachmentMapper attachmentMapper;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return attachments.GetEnumerator();
        }
    }
}
