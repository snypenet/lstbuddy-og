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
    public class Comment
    {
        public Comment()
        {
        }

        public Comment(long commentId)
        {
            this.Load(commentId);
        }

        public void Load(long commentId)
        {
            using (var command = new SqlCommand(SQL.GetCommentById))
            {
                command.Parameters.AddWithValue("@comment_id", commentId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.commentMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            using (var command = new SqlCommand(SQL.SaveComment))
            {
                command.Parameters.AddWithValue("@list_item_id", this.ListItemId);
                command.Parameters.AddWithValue("@text", this.Text);
                command.Parameters.AddWithValue("@created_on", this.CreatedOn);
                command.Parameters.AddWithValue("@created_by", this.CreatedBy.Id);
                command.Parameters.AddWithValue("@is_whisper", this.IsWhisper);

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

            var listItem = new ListItem(ListItemId);
            var list = new List(listItem.ListId);
            var comments = new CommentContainer(ListItemId);
            var idsToIgnore = new List<long>();

            if (IsWhisper)
            {
                idsToIgnore.AddRange(list.Editors.Editors.Select(e => e.Id));
                idsToIgnore.Add(list.Owner.Id);
            }

            idsToIgnore.Add(list.Owner.Id);
            comments.SendCommentResponseEmails(idsToIgnore.ToArray());


            if (!IsWhisper && !comments.Comments.Any(c => c.CreatedBy.Id == list.Owner.Id))
            {
                list.Owner.SendSomeoneCommentedEmail(list.Name);
            }
        }

        public void Delete()
        {
            using (var command = new SqlCommand(SQL.DeleteCommentById))
            {
                command.Parameters.AddWithValue("@comment_id", this.Id);
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
        public string Text { get; set; }
        public Person CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long ListItemId { get; set; }
        public bool IsWhisper { get; set; }

        public void Copy(Comment from)
        {
            this.Id = from.Id;
            this.ListItemId = from.ListItemId;
            this.Text = from.Text;
            this.CreatedBy = from.CreatedBy;
            this.CreatedOn = from.CreatedOn;
            this.IsWhisper = from.IsWhisper;
        }

        private CommentMapper commentMapper = new CommentMapper();
    }
}
