using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearcheyData.Entities;

namespace SearcheyMcSearchface.Models
{
    public class ExploreTagViewModel
    {
        public Tag MainTag { get; set; }
        public List<Tag> RelatedTags { get; set; }
    }
}