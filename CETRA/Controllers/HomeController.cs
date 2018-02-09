using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CETRA.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Default")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize(Roles = "")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}