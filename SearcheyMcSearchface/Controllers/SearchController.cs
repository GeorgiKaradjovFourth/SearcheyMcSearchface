using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SearcheyMcSearchface.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        [ChildActionOnly]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Search/Text
        [ChildActionOnly]
        public ActionResult Search(string text)
        {
            return View();
        }
    }
}
