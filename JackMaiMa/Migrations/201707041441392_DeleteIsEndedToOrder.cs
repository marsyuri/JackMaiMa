namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteIsEndedToOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "IsEnded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "IsEnded", c => c.Boolean(nullable: false));
        }
    }
}
