namespace CCM.Migrations
{
    //fdsfsdfdafsd
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingCodesAndLiaisonCPTRatesRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Liaison_CPTRates", "BillingCodeId", c => c.Int());
            CreateIndex("dbo.Liaison_CPTRates", "BillingCodeId");
            AddForeignKey("dbo.Liaison_CPTRates", "BillingCodeId", "dbo.BillingCodes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Liaison_CPTRates", "BillingCodeId", "dbo.BillingCodes");
            DropIndex("dbo.Liaison_CPTRates", new[] { "BillingCodeId" });
            DropColumn("dbo.Liaison_CPTRates", "BillingCodeId");
        }
    }
}
