using System.Web;
using System.Web.Optimization;

namespace Lists
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js")
                        .Include("~/Scripts/jquery.js")
                        .Include("~/Scripts/jquery.unobtrusive-ajax.js")
                        .Include("~/Scripts/fastclick.js")
                        .Include("~/Scripts/foundation.js")
                        .Include("~/Scripts/grid-auth.js")
                        .Include("~/Scripts/jquery.validate.js")
                        .Include("~/Scripts/jquery.validate.unobtrusive.js")
                        .Include("~/Scripts/Lists.js")
                        .Include("~/Scripts/jquery.datetimepicker.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                        "~/Content/foundation.css",
                        "~/Content/normalize.css",
                        "~/Content/Site.css"));
        }
    }
}