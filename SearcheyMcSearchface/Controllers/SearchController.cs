using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearcheyData;
using SearcheyData.Entities;
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

        private const int SHOW_FIRST_CHARACTERS = 255;
        private const int SHOW_FIRST_DOCUMENTS_COUNT = 20;
        private const int SHOW_FIRST_TAGS_COUNT = 5;

        // GET: Search/Text/
        public ActionResult Search(string text)
        {
            var result = LuceneSearch.Search(text);

            var context = new SearcheyContext();

            SearchViewModel model = new SearchViewModel();
            model.Text = text;
            model.Results = result.Select(s => new SearchResultViewModel
            {
                DocumentId = s.Id,
                Text = s.Text.Length > SHOW_FIRST_CHARACTERS ? s.Text.Substring(0, SHOW_FIRST_CHARACTERS) : s.Text,
                Header = s.Header,
                Source = s.Source,
                Tags = context.Documents.FirstOrDefault(d => d.Id == s.Id).Tags.Take(SHOW_FIRST_TAGS_COUNT).ToList()
            }).Take(SHOW_FIRST_DOCUMENTS_COUNT).ToList();

            return PartialView("_Search", model);
        }

        public ActionResult SearchResult(int id)
        {
            var moreLikeThis = LuceneSearch.MoreLikeThis(id, 5);
            return View();
        }
    }
}
