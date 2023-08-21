namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteCancelAndFinishDateToOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "CancelDate");
            DropColumn("dbo.Orders", "FinishDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "FinishDate", c => c.DateTime());
            AddColumn("dbo.Orders", "CancelDate", c => c.DateTime());
        }
    }
}
