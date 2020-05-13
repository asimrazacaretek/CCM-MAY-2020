namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _BillingCategory_BillingPeriods_Relation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingCategories", "BillingPeriodsId", c => c.Int());
            CreateIndex("dbo.BillingCategories", "BillingPeriodsId");
            AddForeignKey("dbo.BillingCategories", "BillingPeriodsId", "dbo.BillingPeriods", "BillingPeriodsId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillingCategories", "BillingPeriodsId", "dbo.BillingPeriods");
            DropIndex("dbo.BillingCategories", new[] { "BillingPeriodsId" });
            DropColumn("dbo.BillingCategories", "BillingPeriodsId");
        }
    }
}
