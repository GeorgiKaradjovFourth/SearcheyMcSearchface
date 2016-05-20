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
    public class DocumentController : Controller
    {
        // GET: Document/5
        public ActionResult Index(int id = -1)
        {
            if (id == -1)
                return View((DocumentViewModel)null);
            var ctx = new SearcheyContext();
            Document doc = ctx.Documents.SingleOrDefault(d => d.Id == id);
            DocumentViewModel model = new DocumentViewModel()
            {
                Tags = doc.Tags,
                Category = doc.Category,
                Header = doc.Header,
                Id = doc.Id,
                RelatedDocuments = new List<Document>(LuceneSearch.MoreLikeThis(doc.Id, 5)),
                Source = doc.Source,
                Text = doc.Text,
                URL = doc.URL
            };
            
            return View(model);
        }
    }
}
