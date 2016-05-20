using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                    Header = file.Substring(file.LastIndexOf('\\') + 1, file.Length - file.LastIndexOf('\\') - 6),
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

        public ActionResult CleanIndex()
        {
            LuceneSearch.ClearLuceneIndex();
            return null;
        }

        public ActionResult MostImportantTerms()
        {
            var ctx = new SearcheyContext();
            var terms = LuceneSearch.Terms();
            ctx.Tags.AddRange(terms.Select(s => new Tag
            {
                Text = s.Item1.Text,
                TfIdf = s.Item2
            }));
            ctx.SaveChanges();
            return View();
        }

        public ActionResult Colocations()
        {
            var ctx = new SearcheyContext();
            var col = LuceneSearch.AssosiationWithoutDistance();
            foreach (var colocation in col)
            {
                var tag1 = ctx.Tags.Where(t => t.Text == colocation.Item1).FirstOrDefault();
                var tag2 = ctx.Tags.Where(t => t.Text == colocation.Item2).FirstOrDefault();
                if (tag1 != null && tag2 != null)
                {
                    ctx.TagRelations.Add(new TagRelation
                    {
                        Tag1 = tag1,
                        Tag2 = tag2,
                        Relation = colocation.Item3
                    });
                }
            }
            ctx.SaveChanges();
            return null;
        }

        public ActionResult MatchTagsToDocuments()
        {
            var ctx = new SearcheyContext();
            var allDocuments = ctx.Documents.ToList();
            var allTags = ctx.Tags.ToList();

            foreach (var document in allDocuments)
            {
                foreach (var tag in allTags)
                {
                    if (document.Text.Contains(tag.Text))
                    {
                        document.Tags.Add(tag);
                    }
                }
                ctx.SaveChanges();
            }

            return null;
        }
    }
}