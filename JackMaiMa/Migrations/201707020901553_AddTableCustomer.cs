namespace JackMaiMa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableCustomer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
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
                        CreatedBy = c.String(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Stamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Customers");
        }
    }
}
