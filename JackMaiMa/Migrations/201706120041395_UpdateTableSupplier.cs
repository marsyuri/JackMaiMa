namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableSupplier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Suppliers", "CreatedBy", c => c.String());
            AddColumn("dbo.Suppliers", "ModifiedDate", c => c.DateTime());
            AddColumn("dbo.Suppliers", "ModifiedBy", c => c.String());
            AddColumn("dbo.Suppliers", "Stamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Suppliers", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "IsDeleted");
            DropColumn("dbo.Suppliers", "Stamp");
            DropColumn("dbo.Suppliers", "ModifiedBy");
            DropColumn("dbo.Suppliers", "ModifiedDate");
            DropColumn("dbo.Suppliers", "CreatedBy");
            DropColumn("dbo.Suppliers", "CreateDate");
        }
    }
}
