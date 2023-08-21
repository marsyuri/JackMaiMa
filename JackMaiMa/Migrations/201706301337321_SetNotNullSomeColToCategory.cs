namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetNotNullSomeColToCategory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "CreatedBy", c => c.String(nullable: false));
            AlterColumn("dbo.Categories", "ModifiedBy", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "ModifiedBy", c => c.String());
            AlterColumn("dbo.Categories", "CreatedBy", c => c.String());
        }
    }
}
