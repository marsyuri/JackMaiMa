namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteIsSellToStockLog : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StockLogs", "IsSell");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockLogs", "IsSell", c => c.Boolean(nullable: false));
        }
    }
}
