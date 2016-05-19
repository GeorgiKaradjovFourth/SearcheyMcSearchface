using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Novacode;


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
            var files = Directory.GetFiles(@"C:\Users\gkaradjov\Desktop\temp\Process\", "*.docx",
                                         SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                // Create a document in memory:
                var doc = DocX.Load(file);

                // Insert a paragrpah:
                var text = doc.Text;

                Console.WriteLine(text);
            }
            


            return null;
        }
    }
}