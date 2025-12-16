using Lists.Models.Data;
using Lists.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lists.Controllers
{
    public class RegisterController : ControllerBase
    {
        public ActionResult Index()
        {
            LoadViewBag();
            return View(new NewPerson());
        }

        public ActionResult Save(NewPerson person)
        {
            ActionResult result = null;

            if (this.ModelState.IsValid)
            {
                if (Person.DoesUsernameExist(person.Username))
                {
                    this.ModelState.AddModelError("", string.Format("Username {0} already exists", person.Username));
                    LoadViewBag();
                    result = this.PartialView("_Index", person);
                }
                else
                {
                    var registeredPerson = new Person()
                    {
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        MiddleName = person.MiddleName,
                        Username = person.Username
                    };

                    registeredPerson.Save();
                    var contactInfo = new ContactInformation()
                    {
                        CellPhoneNumber = person.CellPhoneNumber,
                        Email = person.Email,
                        PersonId = registeredPerson.Id
                    };

                    if (person.CellPhoneProvider.HasValue)
                    {
                        contactInfo.CellPhoneProvider = new CellPhoneProvider(person.CellPhoneProvider.Value);
                    }

                    contactInfo.Save();
                    registeredPerson.ContactInfo = contactInfo;

                    var credential = new Credentials()
                    {
                        Pattern = person.Pattern,
                        Username = person.Username
                    };

                    credential.Save();

                    this.CurrentUser = registeredPerson;
                    result = Json(Url.Action("Index", "Lists"), JsonRequestBehavior.AllowGet); 
                }
            }
            else
            {
                result = this.PartialView("_Index", person);
                LoadViewBag();
            }

            return result;
        }

        private void LoadViewBag()
        {
            ViewBag.CellPhoneProviders = new SelectList(ContactInformation.CellAllPhoneProviders(), "Id", "Name", "Cell Phone Provider"); 
        }
    }
}
