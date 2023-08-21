namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStockLogTypeIdToStockLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockLogs", "StockLogTypeId", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockLogs", "StockLogTypeId");
        }
    }
}
