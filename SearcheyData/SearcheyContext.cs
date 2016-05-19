using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Document>()
                .HasMany(a => a.Tags)
                .WithMany(t => t.Documents);
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
