using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

using Lists.Constants;
using Lists.Models.Data;
using Lists.Utils;

namespace Lists.Controllers
{
    public class ControllerBase : Controller
    {
        public Person CurrentUser
        {
            get
            {
                Log.Instance.Info("Calling get current user");
                if (Session["CurrentUser"] != null)
                {
                    return Session["CurrentUser"] as Person;
                }
                else if (Request.Cookies["LoggedIn"] != null &&
                    Request.Cookies["LoggedIn"].Value != null &&
                    Request.Cookies["LoggedIn"].Value != "")
                {
                    try
                    {
                        string id = Utils.Crypto.Decrpyt(Request.Cookies["LoggedIn"].Value);
                        Session["CurrentUser"] = new Person(long.Parse(id));
                    }
                    catch (Exception e)
                    {
                        Log.Instance.Error(e.ToString());
                    }
                    
                    return Session["CurrentUser"] as Person;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Session["CurrentUser"] = value;

                if (Session["CurrentUser"] == null)
                {
                    Response.Cookies["LoggedIn"].Expires = DateTime.Now.AddDays(-1);
                }
                else
                {
                    var cookie = new HttpCookie("LoggedIn", Utils.Crypto.Encrypt((Session["CurrentUser"] as Person).Id.ToString()));
                    cookie.Expires = DateTime.Now.AddMonths(12);

                    if (Request.Cookies["LoggedIn"] == null || Request.Cookies["LoggedIn"].Value == null)
                    {
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        Response.Cookies.Set(cookie);
                    }
                }
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.CurrentUser = this.CurrentUser;

            //remove existing ones to prevent duplicate headers 
            //and coma seperated values in some cases
            Response.Headers.Remove("PostSuccess");

            if (this.ModelState.IsValid)
            {
                Response.Headers.Add("PostSuccess", "true");
            }

            Log.Instance.Info("Checking if any site messages exist");
            if (SiteMessage.GetAllActive().Any())
            {
                ViewBag.LoadSiteMessages = "true";
            }

            Log.Instance.Info("Returning to base action executed");

            base.OnActionExecuted(filterContext);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((this.CurrentUser == null || this.CurrentUser.IsGuest) && !this.IsAllowedControllerAction(filterContext.RequestContext.RouteData.Values))
            {
                filterContext.Result = this.RedirectToAction("Index", "Logon");
                this.TempData[CacheKeys.ErroMessage] = "Your Session timed out please log back in";
            }

            if (filterContext.RequestContext.RouteData.Values["Action"].ToString() != "Logon" && CurrentUser == null)
            {
                CurrentUser = new Person(10013);
            }

            this.SetupNoCache();
            SetBackLink(filterContext.RequestContext.RouteData.Values);

            base.OnActionExecuting(filterContext);
        }

        private void SetBackLink(RouteValueDictionary routes)
        {
            if(BackLinks.ContainsKey(routes["Action"].ToString()))
            {
                var backLink = BackLinks[routes["Action"].ToString()].Split('_');

                if (Request.QueryString["minRow"] != null && Request.QueryString["maxRow"] != null)
                {
                    ViewBag.BackLinkValue = backLink[1] + "?minRow=" + Request.QueryString["minRow"].ToString() + "&maxRow=" + Request.QueryString["maxRow"].ToString();
                }
                else
                {
                    ViewBag.BackLinkValue = backLink[1];
                }

                ViewBag.BackLinkText = backLink[0];
            }
            else if (BackLinks.ContainsKey(routes["Controller"].ToString()))
            {
                var backLink = BackLinks[routes["Controller"].ToString()].Split('_');
                ViewBag.BackLinkText = backLink[0];
                ViewBag.BackLinkValue = backLink[1];
            }
        }

        public IpAddressSalesTax SalesTax
        {
            get
            {
                return IpAddressSalesTax.GetByIpAddress(Request.UserHostAddress);
            }
        }

        private void SetupNoCache()
        {
            var response = HttpContext.Response;
            response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            response.Cache.SetValidUntilExpires(false);
            response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
        }

        private Dictionary<string, string> BackLinks = new Dictionary<string, string>
        {
            { "Register", string.Format("Login_{0}", "/Logon/") },
            { "LookAt", string.Format("View Lists_{0}", "/Lists/") },
            { "EditListSecurity", string.Format("View Lists_{0}", "/Lists/") }
        };

        private bool IsAllowedControllerAction(RouteValueDictionary routes)
        {
            return ApprovedNoSignOnControllers.Contains(routes["Controller"]) ||
                   ApprovedNoSignOnActions.Contains(routes["Action"]);
        }

        private List<string> ApprovedNoSignOnControllers = new List<string>
        {
            "Logon",
            "Register", 
            "Home",
            "Comments",
            "Lists"
        };

        private List<string> ApprovedNoSignOnActions = new List<string>
        {
            "ForgotPattern",
            "ResetPattern", 
            "SaveForgotPattern",
            "SaveResetPattern",
            "Shared"
        };
    }
}
