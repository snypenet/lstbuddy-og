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
using Lists.Models.View;

namespace Lists.Models.Data
{
    public class CommentContainer
    {
        public CommentContainer()
        {
        }

        public void SendCommentResponseEmails(params long[] excludePersonIds)
        {
            var list = new List(new ListItem(ListItemId).ListId);
            foreach (var person in comments.Select(item => item.CreatedBy).Where(person => !excludePersonIds.Contains(person.Id)))
            {
                person.SendCommentResponseEmail(list.Name);
            }
        }

        public long ListItemId { get; set; }

        public CommentContainer(long listItemId)
        {
            this.Load(listItemId);
        }

        public void Load(long listItemId)
        {
            ListItemId = listItemId;
            this.comments.Clear();
            using (var command = new SqlCommand(SQL.GetCommentsByListItemId))
            {
                command.Parameters.AddWithValue("@list_item_id", listItemId);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.comments.Add(this.commentMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public List<Comment> Comments
        {
            get { return comments; }
        }

        private List<Comment> comments = new List<Comment>();

        private CommentMapper commentMapper = new CommentMapper();
    }
}
