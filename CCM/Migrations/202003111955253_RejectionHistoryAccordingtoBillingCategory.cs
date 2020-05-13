namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RejectionHistoryAccordingtoBillingCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatusRejectionHistories", "BillingCategoryId", c => c.Int());
            CreateIndex("dbo.CCMCycleStatusRejectionHistories", "BillingCategoryId");
            AddForeignKey("dbo.CCMCycleStatusRejectionHistories", "BillingCategoryId", "dbo.BillingCategories", "BillingCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CCMCycleStatusRejectionHistories", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.CCMCycleStatusRejectionHistories", new[] { "BillingCategoryId" });
            DropColumn("dbo.CCMCycleStatusRejectionHistories", "BillingCategoryId");
        }
    }
}
