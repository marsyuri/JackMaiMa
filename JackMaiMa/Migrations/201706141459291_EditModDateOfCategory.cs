namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditModDateOfCategory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "ModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "ModifiedDate", c => c.DateTime());
        }
    }
}
