namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingCodeAndBillingCYcleRelation : DbMigration
    {
        //fsdfghsadafh
        public override void Up()
        {
            DropForeignKey("dbo.BillingCodesPatients", "BillingCodes_Id", "dbo.BillingCodes");
            DropForeignKey("dbo.BillingCodesPatients", "Patient_Id", "dbo.Patients");
            DropIndex("dbo.BillingCodesPatients", new[] { "BillingCodes_Id" });
            DropIndex("dbo.BillingCodesPatients", new[] { "Patient_Id" });
            CreateTable(
                "dbo.BillingCodesBillingCycles",
                c => new
                    {
                        BillingCodes_Id = c.Int(nullable: false),
                        BillingCycle_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BillingCodes_Id, t.BillingCycle_Id })
                .ForeignKey("dbo.BillingCodes", t => t.BillingCodes_Id, cascadeDelete: true)
                .ForeignKey("dbo.BillingCycles", t => t.BillingCycle_Id, cascadeDelete: true)
                .Index(t => t.BillingCodes_Id)
                .Index(t => t.BillingCycle_Id);
            
            DropTable("dbo.BillingCodesPatients");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BillingCodesPatients",
                c => new
                    {
                        BillingCodes_Id = c.Int(nullable: false),
                        Patient_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BillingCodes_Id, t.Patient_Id });
            
            DropForeignKey("dbo.BillingCodesBillingCycles", "BillingCycle_Id", "dbo.BillingCycles");
            DropForeignKey("dbo.BillingCodesBillingCycles", "BillingCodes_Id", "dbo.BillingCodes");
            DropIndex("dbo.BillingCodesBillingCycles", new[] { "BillingCycle_Id" });
            DropIndex("dbo.BillingCodesBillingCycles", new[] { "BillingCodes_Id" });
            DropTable("dbo.BillingCodesBillingCycles");
            CreateIndex("dbo.BillingCodesPatients", "Patient_Id");
            CreateIndex("dbo.BillingCodesPatients", "BillingCodes_Id");
            AddForeignKey("dbo.BillingCodesPatients", "Patient_Id", "dbo.Patients", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BillingCodesPatients", "BillingCodes_Id", "dbo.BillingCodes", "Id", cascadeDelete: true);
        }
    }
}
