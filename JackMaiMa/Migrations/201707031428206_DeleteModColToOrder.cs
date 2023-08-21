namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteModColToOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "ModifiedBy");
            DropColumn("dbo.Orders", "ModifiedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "ModifiedBy", c => c.String(nullable: false));
        }
    }
}
