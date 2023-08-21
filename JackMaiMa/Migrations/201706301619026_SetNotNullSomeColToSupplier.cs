namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetNotNullSomeColToSupplier : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Suppliers", "CreatedBy", c => c.String(nullable: false));
            AlterColumn("dbo.Suppliers", "ModifiedBy", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Suppliers", "ModifiedBy", c => c.String());
            AlterColumn("dbo.Suppliers", "CreatedBy", c => c.String());
        }
    }
}
