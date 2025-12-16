using Lists.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lists.Models.View;
using Lists.Utils;

namespace Lists.Controllers
{
    public class LogonController : ControllerBase
    {
        public ActionResult Index(string redirectUrl = null)
        {
            Log.Instance.Info("Got to Index Logon");
            if (this.TempData.Peek(CacheKeys.ErroMessage) != null)
            {
                ViewBag.ErrorMessage = TempData[CacheKeys.ErroMessage];
                TempData.Remove(CacheKeys.ErroMessage);
            }

            if (this.TempData.Peek(CacheKeys.SuccessMessage) != null)
            {
                ViewBag.SuccessMessage = TempData[CacheKeys.SuccessMessage];
                TempData.Remove(CacheKeys.SuccessMessage);
            }

            this.TempData["redirectUrl"] = Request.QueryString["redirectUrl"];

            return View(new Credentials());
        }

        public ActionResult Logout()
        {
            CurrentUser = null;
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Logon");
        }

        public ActionResult Login(Credentials credentials)
        {
            ActionResult result = null;

            if (this.ModelState.IsValid)
            {
                var person = credentials.Authenticate();

                if (person == null)
                {
                    this.ModelState.AddModelError("", "Incorrect username or pattern");
                    credentials.Pattern = ""; 
                    result = this.PartialView("_Index", credentials);
                }
                else
                {
                    this.CurrentUser = person;
                    if (this.TempData.Peek("redirectUrl") != null)
                    {
                        result = Json(this.TempData["redirectUrl"].ToString(), JsonRequestBehavior.AllowGet);
                        this.TempData.Remove("redirectUrl");
                    }
                }
            }
            else
            {
                result = this.PartialView("_Index", credentials);
            }

            return result;
        }
    }
}
