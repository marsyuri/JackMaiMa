namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStockLogTypeIdFKToStockLog : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.StockLogs", "StockLogTypeId");
            AddForeignKey("dbo.StockLogs", "StockLogTypeId", "dbo.StockLogTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockLogs", "StockLogTypeId", "dbo.StockLogTypes");
            DropIndex("dbo.StockLogs", new[] { "StockLogTypeId" });
        }
    }
}
