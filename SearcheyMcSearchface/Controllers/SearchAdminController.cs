using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Novacode;
using org.omg.CORBA;
using SearcheyData;
using SearcheyData.Entities;


namespace SearcheyMcSearchface.Controllers
{
    public class SearchAdminController : Controller
    {
        // GET: SearchAdmin
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult MostImportantTerms()
        {
            var terms = TextUtils.GetTfIDFTerms(100, new List<string>());
            return View(terms);
        }

        //TODO: Handle tables and headers
        public ActionResult ImportDocuments()
        {
            var ctx = new SearcheyContext();

            var files = Directory.GetFiles(@"C:\Users\gkaradjov\Desktop\temp\Process\", "*.docx",
                                         SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                // Create a document in memory:
                var doc = DocX.Load(file);

                // Insert a paragrpah:
                var text = doc.Text;
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }
                var document = new Document
                {
                    Header = file.Substring(file.LastIndexOf('\\') + 2),
                    Text = text,
                    Source = SourceType.UserManual
                };
                ctx.Documents.Add(document);
            }
            try
            {
                ctx.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Console.WriteLine();
            }
            

            return null;
        }

        public ActionResult CreateIndex()
        {
            var ctx = new SearcheyContext();
            var docs = ctx.Documents.ToList();

            LuceneSearch.AddUpdateLuceneIndex(docs);

            return null;
        }

        public ActionResult GetMostImportantTerms()
        {
            var ctx = new SearcheyContext();
            var terms = LuceneSearch.Terms();

            return View();
        }
    }
}