using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearcheyData.Entities;

namespace SearcheyData
{
    public class SearcheyContext : DbContext
    {
        public SearcheyContext() : base("DefaultConnection")
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
