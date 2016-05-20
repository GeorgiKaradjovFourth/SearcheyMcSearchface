using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearcheyData.Entities;

namespace SearcheyMcSearchface.Models
{
    public class SearchResultViewModel
    {
        public int DocumentId { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public SourceType Source { get; set; }
        public string URL { get; set; }
        public List<Tag> Tags { get; set; }
    }
}