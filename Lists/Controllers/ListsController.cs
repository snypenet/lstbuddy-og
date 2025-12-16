using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lists.Models.Data;
using Lists.Models.View;
using Lists.Utils;
using System.Web.SessionState;
using System.Configuration;

namespace Lists.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ListsController : ControllerBase
    {
        public ActionResult Index(int minRow = 0, int maxRow = 10)
        {
            return View(ListContainer.GetPage(minRow, maxRow, 10));
        }

        public ActionResult PageListNames(int fromRow, int toRow)
        {
            return PartialView("_ListNames", ListContainer.GetPage(fromRow, toRow, 10));
        }
        
        public ActionResult Add()
        {
            LoadViewBag();
            return View(new NewList());
        }

        public ActionResult Print(long id)
        {
            ViewBag.Title = "Print List";
            return PartialView("_Print", new List(id));
        }
        
        public ActionResult EditList(long id)
        {
            LoadViewBag();
            var list = new List(id);

            var editList = new EditList
            {
                Name = list.Name,
                Id = id,
                ExpiresOn = list.ExpiresOn,
                ListType = list.Type.Id,
                IsPublic = list.IsPublic,
                IsPrivate = list.IsPrivate
            };

            return PartialView("_EditList", editList);
        }


        public ActionResult SaveEditList(EditList editList, int fromRow, int toRow)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var list = new List(editList.Id);

                list.Name = editList.Name;
                list.ExpiresOn = editList.ExpiresOn;
                list.Type = new ListType(editList.ListType);
                list.IsPublic = CurrentUser.IsGuest ? true : editList.IsPublic;
                list.IsPrivate = CurrentUser.IsGuest ? false : editList.IsPrivate;
                list.Update();

                result = PartialView("_ListNames", ListContainer.GetPage(fromRow, toRow, 10));
            }
            else
            {
                LoadViewBag();
                result = PartialView("_EditList", editList);
            }

            return result;
        }

        
        public ActionResult Edit(long id, ListItemType type)
        {
            var item = new ListItem(id);

            ActionResult result = null;

            switch (type)
            {
                case ListItemType.Grocery:
                    result = PartialView("_EditGroceryListItem", new EditGroceryListItem
                    {
                        Id = id,
                        Price = item.Price,
                        Text = item.Text,
                        Url = item.Url,
                        Quantity = item.Quantity.HasValue && item.Quantity.Value > 0 ? item.Quantity.Value : 1
                    });    
                    break;
                case ListItemType.Birthday:
                    result = PartialView("_EditBirthdayListItem", new EditBirthdayListItem
                    {
                        Id = id,
                        Text = item.Text,
                        Url = item.Url
                    });
                    break;
                case ListItemType.Christmas:
                    result = PartialView("_EditChristmasListItem", new EditChristmasListItem
                    {
                        Id = id,
                        Url = item.Url,
                        Text = item.Text
                    });
                    break;
                case ListItemType.PrayerRequest:
                    result = PartialView("_EditPrayerRequestItem", new EditPrayerRequestItem
                    {
                        Id = id,
                        Url = item.Url,
                        Text = item.Text
                    });
                    break;
                case ListItemType.ProsAndCons:
                    result = PartialView("_EditProsAndConsListItem", new EditProsAndConsListItem
                    {
                        Id = id,
                        Url = item.Url,
                        Text = item.Text,
                        Weight = item.Weight.Value,
                        Type = item.ProConType.Value
                    });
                    break;
                case ListItemType.ToDo:
                default:
                    throw new NotImplementedException();
            }

            return result;
        }
        
        public ActionResult DeleteAccess(long id, long PersonId, bool isViewer)
        {
            if (isViewer)
            {
                var viewers = new ViewerContainer(id);
                viewers.Remove(PersonId);
            }
            else
            {
                var editors = new EditorContainer(id);
                editors.Remove(PersonId);
            }

            return null;
        }

        public ActionResult UndeleteAccess(long id, long personId, bool isViewer)
        {
            if (isViewer)
            {
                var viewers = new ViewerContainer(id);
                viewers.Add(personId);
            }
            else
            {
                var editors = new EditorContainer(id);
                editors.Add(personId);
            }

            return null;
        }
        
        public ActionResult EditExistingListSecurity(long id, long personId)
        {
            var editors = new EditorContainer(id);
            var viewers = new ViewerContainer(id);
            ListSecurityType type;

            if (editors.Contains(personId))
            {
                type = ListSecurityType.Edit;
            }
            else if (viewers.Contains(personId))
            {
                type = ListSecurityType.View;
            }
            else
            {
                throw new Exception(string.Format("Person {0} not found as a viewer or editor", personId));
            }

            return PartialView("_EditExistingListSecurity", new EditListSecurity
            {
                ListId = id,
                PersonId = personId,
                AccessType = type
            });
        }

        public ActionResult SearchLists(string query)
        {
            return PartialView("_ListNames", ListContainer.SearchLists(query));
        }

        public ActionResult EditExistingListSecurityRequest(int id)
        {
            var access = new ListAccessRequest(id);

            return PartialView("_EditListAccessRequest", new EditListAccessRequest
            {
                Id = access.Id,
                SecurityType = access.AccessType
            });
        }

        public ActionResult SaveExistingListSecurityRequst(EditListAccessRequest listSecurity)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var accessRequest = new ListAccessRequest(listSecurity.Id);

                accessRequest.List.Editors.Remove(accessRequest.Requestor.Id);
                accessRequest.List.Viewers.Remove(accessRequest.Requestor.Id);

                if (listSecurity.SecurityType == ListSecurityType.Edit)
                {
                    accessRequest.List.Editors.Add(accessRequest.Requestor.Id);
                }
                else
                {
                    accessRequest.List.Viewers.Add(accessRequest.Requestor.Id);
                }


                QueuedEmail.Add(accessRequest.Requestor.ContactInfo.Email,
                    string.Format("Your access request for {0}'s list '{1}' has been approved", accessRequest.List.Owner.FirstName, accessRequest.List.Name),
                    "List Access Approved - Lst Buddy");

                accessRequest.Delete();

                result = PartialView("_EditListSecurity", new ListSecurityContainer(accessRequest.List.Id));
            }
            else
            {
                result = PartialView("_EditListAccessReqest", listSecurity);
            }

            return result;
        }

        public ActionResult ApproveListAccess(int id, bool approved)
        {
            var request = new ListAccessRequest(id);

            if (approved)
            {
                request.List.Editors.Remove(request.Requestor.Id);
                request.List.Viewers.Remove(request.Requestor.Id);

                if (request.AccessType == ListSecurityType.Edit)
                {
                    request.List.Editors.Add(request.Requestor.Id);
                }
                else
                {
                    request.List.Viewers.Add(request.Requestor.Id);
                }

                QueuedEmail.Add(request.Requestor.ContactInfo.Email,
                    string.Format("Your access request for {0}'s list '{1}' has been approved", request.List.Owner.FirstName, request.List.Name),
                    "List Access Approved - Lst Buddy");
            }

            request.Delete();

            return PartialView("_EditListSecurity", new ListSecurityContainer(request.List.Id));
        }
        
        public ActionResult SaveExistingListSecurity(EditListSecurity listSecurity)
        {
            ActionResult result = null;

            if (listSecurity.PersonId == 0)
            {
                ModelState.AddModelError("", "You aren't who you say you are or I don't know who you are");
            }

            if (ModelState.IsValid)
            {
                var list = new List(listSecurity.ListId);
                list.Editors.Remove(listSecurity.PersonId);
                list.Viewers.Remove(listSecurity.PersonId);

                if (listSecurity.AccessType == ListSecurityType.View)
                {
                    list.Viewers.Add(listSecurity.PersonId);
                }
                else
                {
                    list.Editors.Add(listSecurity.PersonId);
                }

                var security = new ListSecurityContainer(listSecurity.ListId);

                result = PartialView("_EditListSecurity", security);
            }
            else
            {
                result = PartialView("_EditExistingListSecurity", listSecurity);
            }

            return result;
        }

        public ActionResult SaveListAccessRequest(NewListSecurity listSecurity)
        {
            ActionResult result = null;

            if (listSecurity.PersonId == 0)
            {
                ModelState.AddModelError("", string.Format("Who are you and what did you do with {0}?", CurrentUser.FirstName));
            }

            if (ModelState.IsValid)
            {
                var list = new List(listSecurity.ListId);
                var request = new ListAccessRequest
                {
                    AccessType = listSecurity.AccessType, 
                    List = list,
                    Requestor = CurrentUser
                };
                request.Save();
                
                string href = string.Format("{0}/Logon", ConfigurationManager.AppSettings["RootUrl"]);
                QueuedEmail.Add(list.Owner.ContactInfo.Email,
                    string.Format("'{0}' has requested access to your list '{1}'. Login <a href='{2}' target='_blank'>here</a> to approve the request.", CurrentUser.FirstName, list.Name, href),
                    "Access Request - Lst Buddy");

                result = Json(Url.Action("EditListSecurity", "Lists", new { id = list.Id }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                result = PartialView("_RequestListAccess", listSecurity);
            }

            return result;
        }
        
        public ActionResult SaveNewListSecurity(NewListSecurity listSecurity)
        {
            ActionResult result = null;

            var list = new List(listSecurity.ListId);

            if(listSecurity.PersonId == list.Owner.Id)
            {
                ModelState.AddModelError("", string.Format("{0} is the owner they don't need to be added as a viewer or editor", list.Owner.FirstName));
            }

            if (listSecurity.PersonId == 0)
            {
                ModelState.AddModelError("", "Select a person to give them access");
            }

            if (ModelState.IsValid)
            {                
                var editors = new EditorContainer(listSecurity.ListId);
                var viewers = new ViewerContainer(listSecurity.ListId);
                viewers.Remove(listSecurity.PersonId);
                editors.Remove(listSecurity.PersonId);

                if (listSecurity.AccessType == ListSecurityType.Edit)
                {
                    editors.Add(listSecurity.PersonId);
                }
                else if(listSecurity.AccessType == ListSecurityType.View)
                {
                    viewers.Add(listSecurity.PersonId);
                }

                result = Json(Url.Action("EditListSecurity", "Lists", new { id = listSecurity.ListId }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                listSecurity.PersonId = 0;
                result = PartialView("_AddListSecurityAccess", listSecurity);
            }

            return result;
        }

        public ActionResult SearchPerson(string query)
        {
            return PartialView("_AutoCompletePersonResult", Person.LookupByName(query));
        }

        
        public ActionResult Claim(long id, ListItemType type, long listId)
        {
            var item = new ListItem(id);
            ActionResult result = null;

            if (item.ClaimedBy == null || !item.ClaimedBy.Contains(CurrentUser.Id))
            {
                item.ClaimedBy.Add(CurrentUser);

                switch (type)
                {
                    case ListItemType.Birthday:
                        result = PartialView("_BirthdayListItems", new ListItemContainer(listId));
                        break;
                    case ListItemType.Christmas:
                        result = PartialView("_ChristmasListItems", new ListItemContainer(listId));
                        break;
                    default:
                        Response.AddHeader("PostError", "Can not claim a " + type.ToString() + " list");
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("PostError", "You've already claimed this item");
            }

            return result;
        }

        
        public ActionResult ProcessGroceryListMarkOff(long id, bool markOff)
        {
            var listItem = new ListItem(id);
            listItem.Purchased = markOff;
            listItem.Update();

            return null;
        }

        
        public ActionResult UnClaim(long id, ListItemType type, long listId)
        {
            var item = new ListItem(id);
            ActionResult result = null;

            if (item.ClaimedBy != null && item.ClaimedBy.Contains(CurrentUser.Id))
            {
                item.ClaimedBy.Remove(CurrentUser);

                switch (type)
                {
                    case ListItemType.Birthday:
                        result = PartialView("_BirthdayListItems", new ListItemContainer(listId));
                        break;
                    case ListItemType.Christmas:
                        result = PartialView("_ChristmasListItems", new ListItemContainer(listId));
                        break;
                    default:
                        Response.AddHeader("PostError", "Can not un-claim a " + type.ToString() + " list item");
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("PostError", "You've already un-claimed this item");
            }

            return result;
        }

        
        public ActionResult SaveBirthdayItem(EditBirthdayListItem item)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var listItem = new ListItem(item.Id);
                listItem.LastModifiedby = CurrentUser;
                listItem.LastModifiedOn = DateTime.Now;

                if (item.Text != listItem.Text)
                {
                    listItem.UnmapAllAmazonitems();
                }

                listItem.Text = item.Text;
                listItem.Url = item.Url;
                listItem.Update();
                result = PartialView("_BirthdayListItems", new ListItemContainer(listItem.ListId));
            }
            else
            {
                result = PartialView("_EditBirthdayListItem", item);
            }

            return result;
        }

        
        public ActionResult SaveGroceryItem(EditGroceryListItem item)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var listItem = new ListItem(item.Id);
                listItem.LastModifiedby = CurrentUser;
                listItem.LastModifiedOn = DateTime.Now;
                listItem.Price = item.Price;
                if (item.Text != listItem.Text)
                {
                    listItem.UnmapAllAmazonitems();
                }
                listItem.Text = item.Text;
                listItem.Url = item.Url;
                listItem.Quantity = item.Quantity.HasValue && item.Quantity.Value > 0 ? item.Quantity.Value : 1;
                listItem.Update();
                result = PartialView("_GroceryListItems", new ListItemContainer(listItem.ListId));
            }
            else
            {
                result = PartialView("_EditGroceryListItem", item);
            }

            return result;
        }

        public ActionResult SaveProAndConItem(EditProsAndConsListItem item)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var listItem = new ListItem(item.Id);
                listItem.LastModifiedby = CurrentUser;
                listItem.LastModifiedOn = DateTime.Now;
                listItem.Text = item.Text;
                listItem.Url = item.Url;
                listItem.ProConType = item.Type;
                listItem.Weight = item.Weight;
                listItem.Update();
                result = PartialView("_ProsAndConsListItems", new ListItemContainer(listItem.ListId));
            }
            else
            {
                result = PartialView("_EditProsAndConsListItem", item);
            }

            return result;
        }

        public ActionResult SaveChristmasItem(EditChristmasListItem item)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var listItem = new ListItem(item.Id);
                listItem.LastModifiedby = CurrentUser;
                listItem.LastModifiedOn = DateTime.Now;

                if (item.Text != listItem.Text)
                {
                    listItem.UnmapAllAmazonitems();
                }

                listItem.Text = item.Text;
                listItem.Url = item.Url;
                listItem.Update();
                result = PartialView("_ChristmasListItems", new ListItemContainer(listItem.ListId));
            }
            else
            {
                result = PartialView("_EditChristmasListItem", item);
            }

            return result;
        }

        public ActionResult AddListSecurityAccess(long id)
        {
            return View(new NewListSecurity
            {
                ListId = id
            });
        }

        public ActionResult RequestListAccess(long id)
        {
            return View(new NewListSecurity
            {
                ListId = id,
                PersonId = CurrentUser.Id
            });

        }

        public ActionResult EditListSecurity(long id)
        {
            return View(new ListSecurityContainer(id));
        }

        public ActionResult DeleteList(long id)
        {
            var list = new List(id);
            list.Delete();
            return null;
        }

        public ActionResult UndoDeleteList(long id)
        {
            List.Undelete(id);
            return null;
        }
        
        public ActionResult CancelAction(long id)
        {
            var listItem = new ListItem(id);
            return RedirectToAction("LookAt", "Lists", new { id = listItem.ListId });
        }

        
        public ActionResult DeleteListItem(long id)
        {
            var listItem = new ListItem(id);
            listItem.Delete();
            return null;
        }

        public ActionResult UnDeleteListItem(long id)
        {
            ListItem.UndoDelete(id);
            return null;
        }
        
        public ActionResult LoadNewItem(long listId, ListItemType itemType)
        {
            ActionResult result = null;

            switch (itemType)
            {
                case ListItemType.Grocery:
                    var allQuickAdds = QuickAddGroceryItem.Get(CurrentUser.Id, true).Concat(QuickAddGroceryItem.Get(CurrentUser.Id));
                    result = PartialView("_NewGroceryListItem", new NewGroceryListItem
                    {
                        ListId = listId,
                        Quantity = 1,
                        QuickAdd = allQuickAdds.Where(g => !g.IsHidden).Select(item => item.ItemName).Distinct()
                    });
                    break;
                case ListItemType.Birthday:
                    result = PartialView("_NewBirthdayListItem", new NewBirthdayListItem
                    {
                        ListId = listId
                    });
                    break;
                case ListItemType.Christmas:
                    result = PartialView("_NewChristmasListItem", new NewChristmasListItem
                    {
                        ListId = listId
                    });
                    break;
                case ListItemType.PrayerRequest:
                    result = PartialView("_NewPrayerRequestitem", new NewPrayerRequestItem
                    {
                        ListId = listId
                    });
                    break;
                case ListItemType.ProsAndCons:
                    result = PartialView("_NewProsAndConsListItem", new NewProsAndConsListItem
                    {
                        ListId = listId
                    });
                    break;
                case ListItemType.ToDo:
                    throw new NotImplementedException();
            }

            return result;
        }

        
        public ActionResult Save(NewList list)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var newList = new List()
                {
                    ExpiresOn = list.ExpiresOn,
                    Name = list.Name,
                    Type = new ListType(list.ListType),
                    Owner = CurrentUser,
                    CreatedOn = DateTime.Now,
                    CreatedBy = CurrentUser,
                    IsPublic = CurrentUser.IsGuest ? true : list.IsPublic,
                    IsPrivate = CurrentUser.IsGuest ? false : list.IsPrivate
                };

                newList.Save();

                if (list.AutoPopulate && newList.Type.Name == "Grocery")
                {
                    newList.AutoPopulate();
                }

                result = Json(Url.Action("Index", "Lists"), JsonRequestBehavior.AllowGet);
            }
            
            return result;
        }
        
        public ActionResult LookAt(long id, int minRow, int maxRow)
        {
            var list = new List(id);
            switch(list.Type.Name.ToLower())
            {
                case "birthday":
                    ViewBag.PartialView = "_Birthday";
                    break;
                case "christmas":
                    ViewBag.PartialView = "_Christmas";
                    break;
                case "grocery":
                    ViewBag.SalesTax = SalesTax;
                    ViewBag.PartialView = "_Groceries";
                    break;
                case "to do":
                    ViewBag.PartialView = "_ToDo";
                    break;
                case "pros and cons":
                    ViewBag.PartialView = "_ProsAndCons";
                    break;
                case "prayer request":
                    ViewBag.PartialView = "_PrayerRequest";
                    break;
                default:
                    throw new Exception(string.Format("{0} is an unknown list type", list.Type.Name)); 
            }

            return View(list);
        }

        private void LoadViewBag()
        {
            ViewBag.ListTypes = new SelectList(ListType.AllListTypes(), "Id", "Name");
        }

        public ActionResult AddProsAndConsItem(NewProsAndConsListItem newListItem)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var item = ListItem.New();
                item.CreatedBy = CurrentUser;
                item.ListId = newListItem.ListId;
                item.Text = newListItem.Text;
                item.Url = newListItem.Url;
                item.LastModifiedby = CurrentUser;
                item.Weight = newListItem.Weight;
                item.ProConType = newListItem.Type;
                item.Save();

                result = PartialView("_ProsAndConsListItems", new ListItemContainer(newListItem.ListId));
            }
            else
            {
                result = PartialView("_NewProsAndConsListItem", newListItem);
            }

            return result;
        }

        public ActionResult Shared(string id)
        {
            var list = new List(id);

            switch (list.Type.Name.ToLower())
            {
                case "birthday":
                    ViewBag.PartialView = "_Birthday";
                    break;
                case "christmas":
                    ViewBag.PartialView = "_Christmas";
                    break;
                case "grocery":
                    ViewBag.SalesTax = SalesTax;
                    ViewBag.PartialView = "_Groceries";
                    break;
                case "to do":
                    ViewBag.PartialView = "_ToDo";
                    break;
                case "pros and cons":
                    ViewBag.PartialView = "_ProsAndCons";
                    break;
                case "prayer request":
                    ViewBag.PartialView = "_PrayerRequest";
                    break;
                default:
                    throw new Exception(string.Format("{0} is an unknown list type", list.Type.Name));
            }
            return View(list);
        }

        public ActionResult SendLeft(long id)
        {
            var item = new ListItem(id);
            item.ProConType = ProOrCon.Con;
            item.Update();
            return PartialView("_ProsAndConsListItems", new ListItemContainer(item.ListId));
        }

        public ActionResult SendRight(long id)
        {
            var item = new ListItem(id);
            item.ProConType = ProOrCon.Pro;
            item.Update();
            return PartialView("_ProsAndConsListItems", new ListItemContainer(item.ListId));
        }
        
        public ActionResult AddBirthdayItem(NewBirthdayListItem newListItem)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var item = ListItem.New();
                item.CreatedBy = CurrentUser;
                item.ListId = newListItem.ListId;
                item.Text = newListItem.Text;
                item.Url = newListItem.Url;
                item.LastModifiedby = CurrentUser;
                item.Save();

                result = PartialView("_BirthdayListItems", new ListItemContainer(newListItem.ListId));
            }
            else
            {
                result = PartialView("_NewBirthdayListItem", newListItem);
            }

            return result;
        }

        public ActionResult IncrementPrayerCount(long id)
        {
            var item = new ListItem(id);
            item.PrayerCount = (item.PrayerCount ?? 0) + 1;
            item.Update();
            return PartialView("_PrayerRequestItems", new ListItemContainer(item.ListId));
        }

        public ActionResult AddPrayerRequest(NewPrayerRequestItem newListItem)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var item = ListItem.New();
                item.CreatedBy = CurrentUser;
                item.ListId = newListItem.ListId;
                item.Text = newListItem.Text;
                item.Url = newListItem.Url;
                item.LastModifiedby = CurrentUser;
                item.Save();

                result = PartialView("_PrayerRequestItems", new ListItemContainer(newListItem.ListId));
            }
            else
            {
                result = PartialView("_NewPrayerRequestItem", newListItem);
            }

            return result;
        }

        
        public ActionResult AddChristmasItem(NewChristmasListItem newListItem)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var item = ListItem.New();
                item.CreatedBy = CurrentUser;
                item.ListId = newListItem.ListId;
                item.Text = newListItem.Text;
                item.Url = newListItem.Url;
                item.LastModifiedby = CurrentUser;
                item.Save();

                result = PartialView("_ChristmasListItems", new ListItemContainer(newListItem.ListId));
            }
            else
            {
                result = PartialView("_NewChristmasListItem", newListItem);
            }

            return result;
        }

        
        public ActionResult AddGroceryItem(NewGroceryListItem newListItem)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var item = ListItem.New();
                item.CreatedBy = CurrentUser;
                item.ListId = newListItem.ListId;
                item.Text = newListItem.Text;
                item.LastModifiedby = CurrentUser;
                item.Quantity = newListItem.Quantity;

                if (!newListItem.Price.HasValue)
                {
                    newListItem.GuessPrice();
                }

                item.Price = newListItem.Price;
                item.Save();

                result = PartialView("_GroceryListItems", new ListItemContainer(newListItem.ListId));
            }
            else
            {
                newListItem.QuickAdd = QuickAddGroceryItem.Get(CurrentUser.Id).Concat(QuickAddGroceryItem.Get(CurrentUser.Id)).Where(g => !g.IsHidden).Select(item => item.ItemName).Distinct();
                result = PartialView("_NewGroceryListItem", newListItem);
            }

            return result;
        }
    }
}
