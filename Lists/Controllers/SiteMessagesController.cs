using Lists.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lists.Controllers
{
    public class SiteMessagesController : Controller
    {
        //
        // GET: /SiteMessages/

        public ActionResult Load()
        {
            return PartialView("_SiteMessages", SiteMessage.GetAllActive());
        }

    }
}
