namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetNotNullSomeColToProduct : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "CreatedBy", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "ModifiedBy", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "ModifiedBy", c => c.String());
            AlterColumn("dbo.Products", "CreatedBy", c => c.String());
        }
    }
}
