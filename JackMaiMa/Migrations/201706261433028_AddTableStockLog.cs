namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableStockLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        OldStock = c.Int(nullable: false),
                        NumberOfChange = c.Int(nullable: false),
                        CurrentStock = c.Int(nullable: false),
                        LogDate = c.DateTime(nullable: false),
                        LogBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockLogs", "ProductId", "dbo.Products");
            DropIndex("dbo.StockLogs", new[] { "ProductId" });
            DropTable("dbo.StockLogs");
        }
    }
}
