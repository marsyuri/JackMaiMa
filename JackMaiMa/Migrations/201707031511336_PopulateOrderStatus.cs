namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateOrderStatus : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO OrderStatus (Id, Name) VALUES (1, 'Processing')");
            Sql("INSERT INTO OrderStatus (Id, Name) VALUES (2, 'Finished')");
            Sql("INSERT INTO OrderStatus (Id, Name) VALUES (3, 'Canceled')");
        }
        
        public override void Down()
        {
        }
    }
}
