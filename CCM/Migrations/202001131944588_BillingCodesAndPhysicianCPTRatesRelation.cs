namespace CCM.Migrations
{
    //fdsfsdfds
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingCodesAndPhysicianCPTRatesRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Physician_CPTRates", "BillingCodeId", c => c.Int());
            CreateIndex("dbo.Physician_CPTRates", "BillingCodeId");
            AddForeignKey("dbo.Physician_CPTRates", "BillingCodeId", "dbo.BillingCodes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Physician_CPTRates", "BillingCodeId", "dbo.BillingCodes");
            DropIndex("dbo.Physician_CPTRates", new[] { "BillingCodeId" });
            DropColumn("dbo.Physician_CPTRates", "BillingCodeId");
        }
    }
}
