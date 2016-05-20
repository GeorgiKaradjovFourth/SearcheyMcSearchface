namespace SearcheyData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatefield : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tag", "TfIdf", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tag", "TfIdf", c => c.Int());
        }
    }
}
