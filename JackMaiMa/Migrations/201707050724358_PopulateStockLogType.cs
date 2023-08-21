namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateStockLogType : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO StockLogTypes (Id, Name) VALUES (1, 'แก้ไขข้อมูล')");
            Sql("INSERT INTO StockLogTypes (Id, Name) VALUES (2, 'ขายสินค้า')");
            Sql("INSERT INTO StockLogTypes (Id, Name) VALUES (3, 'ยกเลิก Order Detail')");
        }
        
        public override void Down()
        {
        }
    }
}
