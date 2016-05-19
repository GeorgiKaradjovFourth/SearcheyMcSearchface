using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearcheyData.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }

        public virtual Tag Parent { get; set; }
        public int? TermFrequency { get; set; }
        public int? TfIdf { get; set; }

        public virtual ICollection<Document> Documents { get; set; } 
    }
}
