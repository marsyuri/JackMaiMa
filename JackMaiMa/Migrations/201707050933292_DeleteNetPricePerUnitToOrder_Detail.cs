namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteNetPricePerUnitToOrder_Detail : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Order_Detail", "NetPricePerDetail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Order_Detail", "NetPricePerDetail", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
