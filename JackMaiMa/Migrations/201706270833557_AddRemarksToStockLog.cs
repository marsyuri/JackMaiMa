namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemarksToStockLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockLogs", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockLogs", "Remarks");
        }
    }
}
