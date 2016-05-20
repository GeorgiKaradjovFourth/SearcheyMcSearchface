using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearcheyMcSearchface.Models;

namespace SearcheyMcSearchface.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        [ChildActionOnly]
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: Search/Text
        public ActionResult Search(string text)
        {
            var result = LuceneSearch.Search(text);

            SearchViewModel model = new SearchViewModel();
            model.Text = text;
            model.Results = result.Select(s => new SearchResultViewModel
            {
                Text = s.Text,
                Header = s.Header,
                Source = s.Source
            }).ToList();

            return PartialView("_Search", model);
        }

        public ActionResult SearchResult(int id)
        {
            var moreLikeThis = LuceneSearch.MoreLikeThis(id, 5);
            return View();
        }
    }
}
