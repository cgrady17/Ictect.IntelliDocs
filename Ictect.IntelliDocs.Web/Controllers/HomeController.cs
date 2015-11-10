using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictect.IntelliDocs.Web.Models;

namespace Ictect.IntelliDocs.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult SplashPage()
        {
            return PartialView("_SplashPage");
        }
    }
}