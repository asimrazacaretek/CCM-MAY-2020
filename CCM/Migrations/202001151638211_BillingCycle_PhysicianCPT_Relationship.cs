namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingCycle_PhysicianCPT_Relationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Physician_CPTRatesBillingCycle",
                c => new
                    {
                        Physician_CPTRates_Id = c.Int(nullable: false),
                        BillingCycle_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Physician_CPTRates_Id, t.BillingCycle_Id })
                .ForeignKey("dbo.Physician_CPTRates", t => t.Physician_CPTRates_Id, cascadeDelete: false)
                .ForeignKey("dbo.BillingCycles", t => t.BillingCycle_Id, cascadeDelete: false)
                .Index(t => t.Physician_CPTRates_Id)
                .Index(t => t.BillingCycle_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Physician_CPTRatesBillingCycle", "BillingCycle_Id", "dbo.BillingCycles");
            DropForeignKey("dbo.Physician_CPTRatesBillingCycle", "Physician_CPTRates_Id", "dbo.Physician_CPTRates");
            DropIndex("dbo.Physician_CPTRatesBillingCycle", new[] { "BillingCycle_Id" });
            DropIndex("dbo.Physician_CPTRatesBillingCycle", new[] { "Physician_CPTRates_Id" });
            DropTable("dbo.Physician_CPTRatesBillingCycle");
        }
    }
}
