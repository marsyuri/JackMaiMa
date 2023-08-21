namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Categories", "CreatedBy", c => c.String());
            AddColumn("dbo.Categories", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Categories", "ModifiedBy", c => c.String());
            AddColumn("dbo.Categories", "Stamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Categories", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "IsDeleted");
            DropColumn("dbo.Categories", "Stamp");
            DropColumn("dbo.Categories", "ModifiedBy");
            DropColumn("dbo.Categories", "ModifiedDate");
            DropColumn("dbo.Categories", "CreatedBy");
            DropColumn("dbo.Categories", "CreateDate");
        }
    }
}
