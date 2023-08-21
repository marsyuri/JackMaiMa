namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRunNoAndEditOrderNoToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "RunNo", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "OrderNo", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "OrderNo", c => c.String());
            DropColumn("dbo.Orders", "RunNo");
        }
    }
}
