namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsSellToStockLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockLogs", "IsSell", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockLogs", "IsSell");
        }
    }
}
