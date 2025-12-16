using Lists.Models.Data;
using Lists.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Lists
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_AcquireRequestState() 
        {
            //if it's china, then redirect
            try
            {
                if (Request.UserHostAddress.StartsWith("180.") || Request.UserHostAddress.StartsWith("202."))
                {
                    Response.Redirect("/Error");
                }
                else
                {
                    var ipAddress = IpAddressSalesTax.GetByIpAddress(Request.UserHostAddress);
                    if (ipAddress.EntryId == 0)
                    {
                        ipAddress.IpAddress = Request.UserHostAddress;
                        ipAddress.Save();
                    }
                }
            }
            catch (Exception e)
            { }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AuthConfig.RegisterAuth();

            ControllerBuilder.Current.SetControllerFactory(typeof(CustomControllerFactory));
            Log.Instance.Info("Application Started");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastException = Server.GetLastError();
            if (lastException.GetType() == typeof(HttpException))
            {
                //nop yet
            }

            Log.Instance.Error(lastException.ToString());

            Response.Redirect("/Error");
        }
    }
}