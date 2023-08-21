namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditModDateOfProduct : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "ModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "ModifiedDate", c => c.DateTime());
        }
    }
}
