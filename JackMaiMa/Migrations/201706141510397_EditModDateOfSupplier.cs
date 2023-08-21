namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditModDateOfSupplier : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Suppliers", "ModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Suppliers", "ModifiedDate", c => c.DateTime());
        }
    }
}
