using Lists.Models.Data;
using Lists.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lists.Controllers
{
    public class CommentsController : ControllerBase
    {
        public ActionResult ListItem(long listItem)
        {
            return PartialView("_ListItemComments", new CommentContainer(listItem));
        }

        public ActionResult PartialListItem(long listItem)
        {
            return PartialView("_PartialListItemComments", new CommentContainer(listItem));
        }

        public ActionResult DeleteComment(long id, bool partial)
        {
            var comment = new Comment(id);
            comment.Delete();
            return PartialView(partial ? "_PartialListItemComments" : "_ListItemComments", new CommentContainer(comment.ListItemId));
        }

        public ActionResult Add(NewComment comment)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var newComment = new Comment
                {
                    CreatedBy = CurrentUser,
                    CreatedOn = DateTime.Now,
                    ListItemId = comment.ListItemId,
                    Text = comment.CommentText,
                    IsWhisper = comment.IsWhisper
                };

                Response.Headers.Add("ListItemId", comment.ListItemId.ToString());

                newComment.Save();

                result = PartialView(comment.IsParial? "_PartialListItemComments" : "_ListItemComments", new CommentContainer(comment.ListItemId));
            }
            else
            {
                result = PartialView("_AddNewComment", comment);
            }

            return result;
        }

        public ActionResult AddNew(long listItemId)
        {
            return PartialView("_AddNewComment", new NewComment
            {
                ListItemId = listItemId
            });
        }
    }
}
