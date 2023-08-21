namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableOrder_DetailAndRelate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Order_Detail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        TotalPricePerDetail = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPerDetail = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPricePerDetail = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.String(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order_Detail", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Order_Detail", "OrderId", "dbo.Orders");
            DropIndex("dbo.Order_Detail", new[] { "ProductId" });
            DropIndex("dbo.Order_Detail", new[] { "OrderId" });
            DropTable("dbo.Order_Detail");
        }
    }
}
