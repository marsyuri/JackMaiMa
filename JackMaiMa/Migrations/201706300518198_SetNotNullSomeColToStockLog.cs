namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetNotNullSomeColToStockLog : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StockLogs", "LogNo", c => c.String(nullable: false));
            AlterColumn("dbo.StockLogs", "LogBy", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StockLogs", "LogBy", c => c.String());
            AlterColumn("dbo.StockLogs", "LogNo", c => c.String());
        }
    }
}
