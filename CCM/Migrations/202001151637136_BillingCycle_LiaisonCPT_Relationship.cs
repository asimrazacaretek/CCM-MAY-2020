namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingCycle_LiaisonCPT_Relationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Liaison_CPTRatesBillingCycle",
                c => new
                    {
                        Liaison_CPTRates_Id = c.Int(nullable: false),
                        BillingCycle_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Liaison_CPTRates_Id, t.BillingCycle_Id })
                .ForeignKey("dbo.Liaison_CPTRates", t => t.Liaison_CPTRates_Id, cascadeDelete: true)
                .ForeignKey("dbo.BillingCycles", t => t.BillingCycle_Id, cascadeDelete: true)
                .Index(t => t.Liaison_CPTRates_Id)
                .Index(t => t.BillingCycle_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Liaison_CPTRatesBillingCycle", "BillingCycle_Id", "dbo.BillingCycles");
            DropForeignKey("dbo.Liaison_CPTRatesBillingCycle", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates");
            DropIndex("dbo.Liaison_CPTRatesBillingCycle", new[] { "BillingCycle_Id" });
            DropIndex("dbo.Liaison_CPTRatesBillingCycle", new[] { "Liaison_CPTRates_Id" });
            DropTable("dbo.Liaison_CPTRatesBillingCycle");
        }
    }
}
