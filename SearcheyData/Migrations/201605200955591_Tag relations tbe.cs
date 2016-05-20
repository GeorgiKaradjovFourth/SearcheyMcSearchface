namespace SearcheyData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tagrelationstbe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagRelation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Relation = c.Double(nullable: false),
                        Tag1_Id = c.Int(),
                        Tag2_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tag", t => t.Tag1_Id)
                .ForeignKey("dbo.Tag", t => t.Tag2_Id)
                .Index(t => t.Tag1_Id)
                .Index(t => t.Tag2_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagRelation", "Tag2_Id", "dbo.Tag");
            DropForeignKey("dbo.TagRelation", "Tag1_Id", "dbo.Tag");
            DropIndex("dbo.TagRelation", new[] { "Tag2_Id" });
            DropIndex("dbo.TagRelation", new[] { "Tag1_Id" });
            DropTable("dbo.TagRelation");
        }
    }
}
