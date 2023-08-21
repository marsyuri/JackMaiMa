namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRunningNoToStockLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockLogs", "RunningNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockLogs", "RunningNo");
        }
    }
}
