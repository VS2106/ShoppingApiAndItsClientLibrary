namespace ShoppingAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDomainRelatedTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblOrderItem",
                c => new
                    {
                        intOrderItemId = c.Int(nullable: false, identity: true),
                        intProductId = c.Int(nullable: false),
                        intQuantity = c.Int(nullable: false),
                        Order_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.intOrderItemId)
                .ForeignKey("dbo.tblProduct", t => t.intProductId, cascadeDelete: true)
                .ForeignKey("dbo.tblOrder", t => t.Order_Id, cascadeDelete: true)
                .Index(t => t.intProductId)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.tblProduct",
                c => new
                    {
                        intProductId = c.Int(nullable: false, identity: true),
                        strName = c.String(nullable: false, maxLength: 200),
                        intStockQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.intProductId);
            
            CreateTable(
                "dbo.tblOrder",
                c => new
                    {
                        intOrderId = c.Int(nullable: false, identity: true),
                        strIdentityUserId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.intOrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblOrderItem", "Order_Id", "dbo.tblOrder");
            DropForeignKey("dbo.tblOrderItem", "intProductId", "dbo.tblProduct");
            DropIndex("dbo.tblOrderItem", new[] { "Order_Id" });
            DropIndex("dbo.tblOrderItem", new[] { "intProductId" });
            DropTable("dbo.tblOrder");
            DropTable("dbo.tblProduct");
            DropTable("dbo.tblOrderItem");
        }
    }
}
