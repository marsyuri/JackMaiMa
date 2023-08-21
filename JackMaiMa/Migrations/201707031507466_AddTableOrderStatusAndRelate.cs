namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableOrderStatusAndRelate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderStatus",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Orders", "OrderStatusId");
            AddForeignKey("dbo.Orders", "OrderStatusId", "dbo.OrderStatus", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "OrderStatusId", "dbo.OrderStatus");
            DropIndex("dbo.Orders", new[] { "OrderStatusId" });
            DropTable("dbo.OrderStatus");
        }
    }
}
