namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnrollmentSubstatusReason_BillingCategory_relationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EnrollmentSubstatusReasons", "BillingCategoryId", c => c.Int());
            CreateIndex("dbo.EnrollmentSubstatusReasons", "BillingCategoryId");
            AddForeignKey("dbo.EnrollmentSubstatusReasons", "BillingCategoryId", "dbo.BillingCategories", "BillingCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EnrollmentSubstatusReasons", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.EnrollmentSubstatusReasons", new[] { "BillingCategoryId" });
            DropColumn("dbo.EnrollmentSubstatusReasons", "BillingCategoryId");
        }
    }
}
