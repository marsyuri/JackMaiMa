namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHasImageToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "HasImage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "HasImage");
        }
    }
}
