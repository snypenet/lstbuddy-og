using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Dashboard;
using Lists.Models.Data;

namespace Lists
{
    public class ListsAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line.
            //var context = new OwinContext(owinEnvironment);
            if (HttpContext.Current.Request.Cookies["LoggedIn"] != null &&
                    HttpContext.Current.Request.Cookies["LoggedIn"].Value != null &&
                    HttpContext.Current.Request.Cookies["LoggedIn"].Value != "")
            {
                string id = Utils.Crypto.Decrpyt(HttpContext.Current.Request.Cookies["LoggedIn"].Value);
                var person = new Person(long.Parse(id));
                if (person.IsAdmin)
                {
                    return true;
                }
            }

            HttpContext.Current.Response.Redirect("/Logon?redirectUrl=/hangfire", true);
            return false;
        }
    }
}