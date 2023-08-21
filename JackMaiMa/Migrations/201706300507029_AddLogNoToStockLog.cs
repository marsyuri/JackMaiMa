namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogNoToStockLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockLogs", "LogNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockLogs", "LogNo");
        }
    }
}
