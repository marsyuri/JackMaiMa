namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeColToOrder1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ModifiedBy", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "CancelDate", c => c.DateTime());
            AddColumn("dbo.Orders", "FinishDate", c => c.DateTime());
            AddColumn("dbo.Orders", "IsEnded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsEnded");
            DropColumn("dbo.Orders", "FinishDate");
            DropColumn("dbo.Orders", "CancelDate");
            DropColumn("dbo.Orders", "ModifiedDate");
            DropColumn("dbo.Orders", "ModifiedBy");
        }
    }
}
