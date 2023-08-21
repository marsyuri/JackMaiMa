namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteIsDeletedToOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "IsDeleted", c => c.Boolean(nullable: false));
        }
    }
}
