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
            List<SearchResultViewModel> results = new List<SearchResultViewModel>();

            List<string> tags = new List<string>();
            tags.Add("Tag1");
            tags.Add("Tag2");
            tags.Add("Cool Tag");
            SearchResultViewModel example = new SearchResultViewModel()
            {
                Header = "Text",
                Text = "McTexty",
                URL = "testUrl",
                Tags = tags
            };
            results.Add(example);

            SearchViewModel model = new SearchViewModel();
            model.Text = text;
            model.Results = results;

            return PartialView("_Search", model);
        }
    }
}
