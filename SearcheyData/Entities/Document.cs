using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearcheyData.Entities
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        public string Header { get; set; }

        [Required]
        public string Text { get; set; }

        public string URL { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public Tag Category { get; set; }

        public SourceType Source { get; set; }
    }
}
