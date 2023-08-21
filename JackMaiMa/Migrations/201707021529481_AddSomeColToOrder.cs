namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeColToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CreatedBy", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "ModifiedBy", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "Stamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Orders", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsDeleted");
            DropColumn("dbo.Orders", "Stamp");
            DropColumn("dbo.Orders", "ModifiedDate");
            DropColumn("dbo.Orders", "ModifiedBy");
            DropColumn("dbo.Orders", "CreateDate");
            DropColumn("dbo.Orders", "CreatedBy");
        }
    }
}
