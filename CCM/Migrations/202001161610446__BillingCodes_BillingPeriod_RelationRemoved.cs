namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _BillingCodes_BillingPeriod_RelationRemoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BillingCodes", "BillingPeriodsId", "dbo.BillingPeriods");
            DropIndex("dbo.BillingCodes", new[] { "BillingPeriodsId" });
            DropColumn("dbo.BillingCodes", "BillingPeriodsId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BillingCodes", "BillingPeriodsId", c => c.Int(nullable: false));
            CreateIndex("dbo.BillingCodes", "BillingPeriodsId");
            AddForeignKey("dbo.BillingCodes", "BillingPeriodsId", "dbo.BillingPeriods", "BillingPeriodsId", cascadeDelete: true);
        }
    }
}
