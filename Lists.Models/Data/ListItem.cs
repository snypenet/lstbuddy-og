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
    public class ListItem
    {
        private ListItemMapper listItemMapper = new ListItemMapper();

        public static ListItem New()
        {
            var item = new ListItem
            {
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now
            };
            return item;
        }

        public void UnmapAllAmazonitems()
        {
            foreach (var item in AmazonItems)
            {
                item.UnMapFrom(Id);
            }
        }

        private List<AmazonItem> amazonItems;
        public List<AmazonItem> AmazonItems
        {
            get
            {
                if (amazonItems == null)
                {
                    amazonItems = AmazonItem.GetItems(Id);
                }

                return amazonItems;
            }
        }

        public ListItem()
        {
        }

        public static void UndoDelete(long id)
        {
            using (var command = new SqlCommand(SQL.UndeleteListItemById))
            {
                command.Parameters.AddWithValue("@list_item_id", id);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }

        public ListItem(long listItemId)
        {
            this.Load(listItemId);
        }

        public void Load(long listItemId)
        {
            using (var command = new SqlCommand(SQL.GetListItemById))
            {
                command.Parameters.AddWithValue("@list_item_id", listItemId);

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Copy(this.listItemMapper.Convert(reader));
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            using (var command = new SqlCommand(SQL.SaveListItem))
            {
                command.Parameters.AddWithValue("@list_Id", this.ListId);
                command.Parameters.AddWithValue("@list_item_content", this.Text);
                command.Parameters.AddWithValue("@created_on", this.CreatedOn);
                command.Parameters.AddWithValue("@created_by", this.CreatedBy.Id);
                command.Parameters.AddWithValue("@last_modified_by", this.LastModifiedby.Id);
                command.Parameters.AddWithValue("@last_modified_on", this.LastModifiedOn);

                if (this.Quantity.HasValue)
                {
                    command.Parameters.AddWithValue("@quantity", this.Quantity.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@quantity", DBNull.Value);
                }

                if (this.Price.HasValue)
                {
                    command.Parameters.AddWithValue("@grocery_price", this.Price.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@grocery_price", DBNull.Value);
                }

                if (this.Purchased.HasValue)
                {
                    command.Parameters.AddWithValue("@purchased", this.Purchased.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@purchased", DBNull.Value);
                }

                if (PrayerCount.HasValue)
                {
                    command.Parameters.AddWithValue("@prayer_count", this.PrayerCount.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@prayer_count", DBNull.Value);
                }

                if (Url != null)
                {
                    command.Parameters.AddWithValue("@url", Url);
                }
                else
                {
                    command.Parameters.AddWithValue("@url", DBNull.Value);
                }

                if (Weight.HasValue)
                {
                    command.Parameters.AddWithValue("@pro_con_weight", (int)Weight.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@pro_con_weight", DBNull.Value);
                }

                if (ProConType.HasValue)
                {
                    command.Parameters.AddWithValue("@pro_con_type", (int)ProConType.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@pro_con_type", DBNull.Value);
                }

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
                }
            }

            var list = new List(ListId);

            foreach (var people in new EditorContainer(ListId).Editors.Concat(new ViewerContainer(ListId).Viewers))
            {
                people.SendNewListItemEmail(list.Name);
            }

        }

        public void Update()
        {
            using (var command = new SqlCommand(SQL.UpdateListItemById))
            {
                command.Parameters.AddWithValue("@list_item_id", this.Id);
                command.Parameters.AddWithValue("@list_id", this.ListId);
                command.Parameters.AddWithValue("@list_item_content", this.Text);
                command.Parameters.AddWithValue("@last_modified_by", this.LastModifiedby.Id);
                command.Parameters.AddWithValue("@last_modified_on", this.LastModifiedOn);

                if (this.Quantity.HasValue)
                {

                    command.Parameters.AddWithValue("@quantity", this.Quantity.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@quantity", DBNull.Value);
                }

                if (this.Price.HasValue)
                {
                    command.Parameters.AddWithValue("@grocery_price", this.Price.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@grocery_price", DBNull.Value);
                }

                if (this.Purchased.HasValue)
                {
                    command.Parameters.AddWithValue("@purchased", this.Purchased.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@purchased", DBNull.Value);
                }

                if (PrayerCount.HasValue)
                {
                    command.Parameters.AddWithValue("@prayer_count", this.PrayerCount.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@prayer_count", DBNull.Value);
                }

                if (Url != null)
                {
                    command.Parameters.AddWithValue("@url", Url);
                }
                else
                {
                    command.Parameters.AddWithValue("@url", DBNull.Value);
                }

                if (Weight.HasValue)
                {
                    command.Parameters.AddWithValue("@pro_con_weight", (int)Weight.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@pro_con_weight", DBNull.Value);
                }

                if (ProConType.HasValue)
                {
                    command.Parameters.AddWithValue("@pro_con_type", (int)ProConType.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@pro_con_type", DBNull.Value);
                }

                using (SqlConnection databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
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
            using (var command = new SqlCommand(SQL.DeleteListItemById))
            {
                command.Parameters.AddWithValue("@list_item_id", this.Id);

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
        public long ListId { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// Used for grocery list item
        /// </summary>
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public bool? Purchased { get; set; }
        public string Url { get; set; }

        public DateTime CreatedOn { get; set; }
        public Person CreatedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public Person LastModifiedby { get; set; }

        public ProConWeight? Weight { get; set; }
        public ProOrCon? ProConType { get; set; }

        public int Amount
        {
            get
            {
                int i = 0;

                if (ProConType.HasValue && Weight.HasValue)
                {
                    i = ProConType.Value == ProOrCon.Pro ? (int)Weight.Value : -1 * (int)Weight.Value;
                }

                return i;
            }
        }

        public long? PrayerCount { get; set; }

        private ClaimedByContainer claimedBy;
        public ClaimedByContainer ClaimedBy 
        {
            get
            {
                if (claimedBy == null)
                {
                    claimedBy = new ClaimedByContainer(Id);
                }
                return claimedBy;
            }
            set
            {
                claimedBy = value;
            }
        }

        private CommentContainer comments;
        public CommentContainer Comments 
        {
            get
            {
                if (comments == null)
                {
                    comments = new CommentContainer(Id);
                }
                return comments;
            }
            set
            {
                comments = value;
            }
        }

        public void Copy(ListItem from)
        {
            this.CreatedBy = from.CreatedBy;
            this.CreatedOn = from.CreatedOn;
            this.Id = from.Id;
            this.Text = from.Text;
            this.LastModifiedby = from.LastModifiedby;
            this.LastModifiedOn = from.LastModifiedOn;
            this.ListId = from.ListId;
            this.ClaimedBy = from.ClaimedBy;
            this.Comments = from.Comments;
            this.Price = from.Price;
            this.Quantity = from.Quantity;
            this.Purchased = from.Purchased;
            this.PrayerCount = from.PrayerCount;
            this.Url = from.Url;
            this.ProConType = from.ProConType;
            this.Weight = from.Weight;
        }
    }
}
