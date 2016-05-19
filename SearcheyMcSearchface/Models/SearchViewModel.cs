using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearcheyMcSearchface.Models
{
    public class SearchViewModel
    {
        public string Text { get; set; }

        public List<SearchResult> Results { get; set; }
    }
}