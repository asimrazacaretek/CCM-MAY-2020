namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewTimeCycle_billingCategory_Changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReviewTimeCcms", "RollupFrom", c => c.String());
            AddColumn("dbo.ReviewTimeCcms", "RolledupTo", c => c.String());
            AddColumn("dbo.ReviewTimeCcms", "IsLocked", c => c.Boolean(nullable: false));
            AddColumn("dbo.ReviewTimeCcms", "BillingcategoryId", c => c.Int());
            AddColumn("dbo.ReviewTimeCcms", "BillingCodeId", c => c.Int());
            CreateIndex("dbo.ReviewTimeCcms", "BillingcategoryId");
            AddForeignKey("dbo.ReviewTimeCcms", "BillingcategoryId", "dbo.BillingCategories", "BillingCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReviewTimeCcms", "BillingcategoryId", "dbo.BillingCategories");
            DropIndex("dbo.ReviewTimeCcms", new[] { "BillingcategoryId" });
            DropColumn("dbo.ReviewTimeCcms", "BillingCodeId");
            DropColumn("dbo.ReviewTimeCcms", "BillingcategoryId");
            DropColumn("dbo.ReviewTimeCcms", "IsLocked");
            DropColumn("dbo.ReviewTimeCcms", "RolledupTo");
            DropColumn("dbo.ReviewTimeCcms", "RollupFrom");
        }
    }
}
