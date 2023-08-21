namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 1024),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberInStock = c.Int(nullable: false),
                        NumberInSell = c.Int(nullable: false),
                        IsNoImage = c.Boolean(nullable: false),
                        ImageUrl = c.String(maxLength: 1024),
                        CategoryId = c.Int(nullable: false),
                        SupplierId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        Stamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        ContactName = c.String(nullable: false, maxLength: 100),
                        ContactTitle = c.String(maxLength: 100),
                        Address = c.String(),
                        Tambon = c.String(),
                        Amphur = c.String(),
                        Province = c.String(),
                        PostalCode = c.String(maxLength: 5),
                        Phone = c.String(),
                        Email = c.String(),
                        OptionalContact = c.String(maxLength: 512),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Products", new[] { "SupplierId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropTable("dbo.Suppliers");
            DropTable("dbo.Products");
        }
    }
}
