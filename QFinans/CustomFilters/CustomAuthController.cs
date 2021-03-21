using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QFinans.CustomFilters
{
    public class CustomAuthController : Controller
    {
        // GET: CustomAuth
        public ActionResult UnAuthorized()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}