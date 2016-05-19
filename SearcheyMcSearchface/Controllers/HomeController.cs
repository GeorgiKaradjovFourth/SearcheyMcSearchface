using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearcheyData;
using SearcheyData.Entities;

namespace SearcheyMcSearchface.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var ctx = new SearcheyContext();
            ctx.Documents.Add(new Document() {Header = "Test", Text = "Test Text"});
            ctx.SaveChanges();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}