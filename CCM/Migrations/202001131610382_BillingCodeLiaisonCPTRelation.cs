namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingCodeLiaisonCPTRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Liaison_CPTRates", "BillingId", c => c.Int());
            CreateIndex("dbo.Liaison_CPTRates", "BillingId");
            AddForeignKey("dbo.Liaison_CPTRates", "BillingId", "dbo.BillingCodes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Liaison_CPTRates", "BillingId", "dbo.BillingCodes");
            DropIndex("dbo.Liaison_CPTRates", new[] { "BillingId" });
            DropColumn("dbo.Liaison_CPTRates", "BillingId");
        }
    }
}
