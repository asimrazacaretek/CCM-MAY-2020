namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _BillingCodes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillingCategories",
                c => new
                    {
                        BillingCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.BillingCategoryId);
            
            CreateTable(
                "dbo.BillingCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MinimunMinutes = c.Double(nullable: false),
                        BillingCategoryId = c.Int(nullable: false),
                        BillingPeriodsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCategories", t => t.BillingCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.BillingPeriods", t => t.BillingPeriodsId, cascadeDelete: true)
                .Index(t => t.BillingCategoryId)
                .Index(t => t.BillingPeriodsId);
            
            CreateTable(
                "dbo.BillingPeriods",
                c => new
                    {
                        BillingPeriodsId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.BillingPeriodsId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillingCodes", "BillingPeriodsId", "dbo.BillingPeriods");
            DropForeignKey("dbo.BillingCodes", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.BillingCodes", new[] { "BillingPeriodsId" });
            DropIndex("dbo.BillingCodes", new[] { "BillingCategoryId" });
            DropTable("dbo.BillingPeriods");
            DropTable("dbo.BillingCodes");
            DropTable("dbo.BillingCategories");
        }
    }
}
