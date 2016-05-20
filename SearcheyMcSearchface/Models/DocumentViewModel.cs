using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearcheyData.Entities;

namespace SearcheyMcSearchface.Models
{
    public class DocumentViewModel : Document
    {
        public List<Document> RelatedDocuments { get; set; } 
    }
}