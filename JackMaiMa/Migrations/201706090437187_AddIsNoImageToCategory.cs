namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsNoImageToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "IsNoImage", c => c.Boolean(nullable: false));
            DropColumn("dbo.Categories", "HasImage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Categories", "HasImage", c => c.Boolean(nullable: false));
            DropColumn("dbo.Categories", "IsNoImage");
        }
    }
}
