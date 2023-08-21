namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderStatusIdToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderStatusId", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderStatusId");
        }
    }
}
