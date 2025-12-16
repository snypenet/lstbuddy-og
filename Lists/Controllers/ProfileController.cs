using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lists.Models.View;
using Lists.Models.Data;
using Lists.Models.Utils;
using Lists.Constants;

namespace Lists.Controllers
{
    public class ProfileController : ControllerBase
    {
        public ActionResult Index()
        {
            var information = new ProfileInformation
            {
                PersonId = CurrentUser.Id,
                ContactInformation = new ExistingContactInformation
                {
                    CellPhoneNumber = CurrentUser.ContactInfo.CellPhoneNumber,
                    CellPhoneProvider = CurrentUser.ContactInfo.CellPhoneProvider,
                    Email = CurrentUser.ContactInfo.Email
                },
                Credentials = new ExistingCredentials 
                {
                    Username = CurrentUser.Username
                }, 
                QuickAddItems = QuickAddGroceryItem.Get(CurrentUser.Id, true),
                AutoQuickAddItems = QuickAddGroceryItem.Get(CurrentUser.Id, false)
            };

            return View(information);
        }

        public ActionResult ResetPattern(string id)
        {
            var change = new ChangeControl(id.ToString());

            ActionResult result = null;

            if (!change.IsValid || change.Used)
            {
                TempData[CacheKeys.ErroMessage] = "This URL has expired attempt to reset pattern again.";
                result = RedirectToAction("Index", "Logon");
            }
            else
            {
                result = View(new ResetPatternControl
                {
                    ControlId = id
                });
            }

            return result;
        }

        public ActionResult SaveResetPattern(ResetPatternControl resetPattern)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var change = new ChangeControl(resetPattern.ControlId);

                if (!change.IsValid || change.Used)
                {
                    TempData[CacheKeys.ErroMessage] = "Your reset pattern expired please try again later.";
                    result = RedirectToAction("Index", "Logon");
                }
                else
                {
                    var person = Person.GetPersonByEmailAndUsername(resetPattern.Email, resetPattern.Username);

                    if (person != null)
                    {
                        var credentials = new Credentials
                        {
                            Pattern = resetPattern.Pattern,
                            Username = resetPattern.Username
                        };

                        credentials.ReplacePattern();
                        change.MarkUsed();
                        TempData[CacheKeys.SuccessMessage] = "You reset your pattern successfully";
                        result = Json(Url.Action("Index", "Logon"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or email is invalid");
                        resetPattern.Pattern = "";
                        result = PartialView("_ResetPattern", resetPattern);
                    }
                }
            }
            else
            {
                resetPattern.Pattern = "";
                result = PartialView("_ResetPattern", resetPattern);
            }

            return result;
        }

        public ActionResult ForgotPattern()
        {
            return View();
        }

        public ActionResult SaveForgotPattern(ForgotPattern forgotPattern)
        {
            ActionResult result = null;

            if (this.ModelState.IsValid)
            {
                var person = Person.GetPersonByEmailAndUsername(forgotPattern.Email, forgotPattern.Username);

                if (person != null)
                {
                    person.SendForgotPatternEmail();
                    TempData[CacheKeys.SuccessMessage] = "Reset email has been sent please check you inbox and spam folder to reset your pattern";
                    result = Json(Url.Action("Index", "Logon"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Username or Email is incorrect");
                    result = this.PartialView("_ForgotPattern", forgotPattern);
                }
            }
            else
            {
                result = this.PartialView("_ForgotPattern", forgotPattern);
            }

            return result;
        }

        public ActionResult SaveCredentials(EditCredentials credentials)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var person = CurrentUser;

                if (person.Username != credentials.Username)
                {
                    person.Username = credentials.Username;
                    person.Update();
                }

                var currentCredentials = new Credentials();
                currentCredentials.Username = credentials.Username;
                currentCredentials.Pattern = credentials.Pattern;
                currentCredentials.ReplacePattern();

                result = PartialView("_CredentialsView", new ExistingCredentials
                {
                    Username = person.Username
                });
            }
            else
            {
                result = PartialView("_CredentialsEdit", credentials);
            }

            return result;
        }

        public ActionResult AddNewQuickAddItem(NewQuickAddGroceryItem newItem)
        {
            var item = new QuickAddGroceryItem
            {
                PersonId = CurrentUser.Id,
                ItemName = newItem.ItemName,
                UserEntered = true
            };
            item.Save();
            return PartialView("_QuickGroceryItemView", QuickAddGroceryItem.Get(CurrentUser.Id, true));
        }

        public ActionResult DeleteQuickAddItem(int itemId)
        {
            var item = new QuickAddGroceryItem(itemId);
            item.Delete();
            return null;
        }

        public ActionResult UndeleteQuickAddItem(string itemName)
        {
            var item = new QuickAddGroceryItem
            {
                ItemName = itemName, 
                UserEntered = true,
                PersonId = CurrentUser.Id
            };
            item.Save();
            return null;
        }

        public ActionResult HideQuickAddItem(int itemId)
        {
            var item = new QuickAddGroceryItem(itemId);
            item.IsHidden = true;
            item.Update();
            return null;
        }

        public ActionResult UnhideQuickAddItem(int itemId)
        {
            var item = new QuickAddGroceryItem(itemId);
            item.IsHidden = true;
            item.Update();
            return null;
        }

        public ActionResult SaveContactInformation(EditContactInformation contactInformation)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                var person = CurrentUser;

                if (contactInformation.CellPhoneProvider.HasValue)
                {
                    person.ContactInfo.CellPhoneProvider = new CellPhoneProvider(contactInformation.CellPhoneProvider.Value);
                }
                else
                {
                    person.ContactInfo.CellPhoneProvider = null;
                }

                person.ContactInfo.CellPhoneNumber = contactInformation.CellPhoneNumber;
                person.ContactInfo.Email = contactInformation.Email;
                person.ContactInfo.Update();

                result = PartialView("_ContactInformationView", new ExistingContactInformation
                {
                    CellPhoneNumber = person.ContactInfo.CellPhoneNumber,
                    CellPhoneProvider = person.ContactInfo.CellPhoneProvider,
                    Email = person.ContactInfo.Email
                });
            }
            else
            {
                LoadViewBag();
                result = PartialView("_ContactInformationEdit", contactInformation);
            }

            return result;
        }

        private void LoadViewBag()
        {
            ViewBag.CellPhoneProviders = new SelectList(ContactInformation.CellAllPhoneProviders(), "Id", "Name", "Cell Phone Provider");
        }

        public ActionResult LoadEdit(string type)
        {
            ActionResult result = null;

            if (type == "creds")
            {
                result = PartialView("_CredentialsEdit", new EditCredentials
                {
                    Username = CurrentUser.Username
                });
            }
            else
            {
                LoadViewBag();
                var contactInformation = new EditContactInformation
                {
                    CellPhoneNumber = CurrentUser.ContactInfo.CellPhoneNumber, 
                    Email = CurrentUser.ContactInfo.Email
                };

                if(CurrentUser.ContactInfo.CellPhoneProvider != null && CurrentUser.ContactInfo.CellPhoneProvider.Id != 0)
                {
                    contactInformation.CellPhoneProvider = CurrentUser.ContactInfo.CellPhoneProvider.Id;
                }

                result = PartialView("_ContactInformationEdit", contactInformation);
            }

            return result;
        }

        public ActionResult LoadView(string type)
        {
            ActionResult result = null;

            if (type == "creds")
            {
                result = PartialView("_CredentialsView", new ExistingCredentials
                {
                    Username = CurrentUser.Username
                });
            }
            else
            {
                result = PartialView("_ContactInformationView", new ExistingContactInformation
                {
                    CellPhoneNumber = CurrentUser.ContactInfo.CellPhoneNumber,
                    CellPhoneProvider = CurrentUser.ContactInfo.CellPhoneProvider,
                    Email = CurrentUser.ContactInfo.Email
                });
            }

            return result;
        }
    }
}
