namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _FullBillingCode1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Liaisons", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates");
            DropForeignKey("dbo.Physicians", "Physician_CPTRates_Id", "dbo.Physician_CPTRates");
            DropIndex("dbo.Liaisons", new[] { "Liaison_CPTRates_Id" });
            DropIndex("dbo.Physicians", new[] { "Physician_CPTRates_Id" });
            CreateTable(
                "dbo.BillingCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MinimunMinutes = c.Double(nullable: false),
                        BillingCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCategories", t => t.BillingCategoryId, cascadeDelete: true)
                .Index(t => t.BillingCategoryId);
            
            CreateTable(
                "dbo.BillingCategories",
                c => new
                    {
                        BillingCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        BillingPeriodsId = c.Int(),
                    })
                .PrimaryKey(t => t.BillingCategoryId)
                .ForeignKey("dbo.BillingPeriods", t => t.BillingPeriodsId)
                .Index(t => t.BillingPeriodsId);
            
            CreateTable(
                "dbo.BillingPeriods",
                c => new
                    {
                        BillingPeriodsId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.BillingPeriodsId);
            
            CreateTable(
                "dbo.Patients_BillingCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(),
                        BillingCategoryId = c.Int(),
                        Status = c.Boolean(nullable: false),
                        CycleId = c.Int(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        EnrolledOn = c.DateTime(),
                        DeEnrolledOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCategories", t => t.BillingCategoryId)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.BillingCategoryId);
            
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
            
            CreateTable(
                "dbo.Physician_CPTRatesBillingCycle",
                c => new
                    {
                        Physician_CPTRates_Id = c.Int(nullable: false),
                        BillingCycle_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Physician_CPTRates_Id, t.BillingCycle_Id })
                .ForeignKey("dbo.Physician_CPTRates", t => t.Physician_CPTRates_Id, cascadeDelete: true)
                .ForeignKey("dbo.BillingCycles", t => t.BillingCycle_Id, cascadeDelete: true)
                .Index(t => t.Physician_CPTRates_Id)
                .Index(t => t.BillingCycle_Id);
            
            AddColumn("dbo.Liaison_CPTRates", "BillingCodeId", c => c.Int());
            AddColumn("dbo.Physician_CPTRates", "BillingCodeId", c => c.Int());
            AlterColumn("dbo.Liaison_CPTRates", "LiaisonId", c => c.Int());
            AlterColumn("dbo.Physician_CPTRates", "PhysicianId", c => c.Int());
            CreateIndex("dbo.Liaison_CPTRates", "LiaisonId");
            CreateIndex("dbo.Liaison_CPTRates", "BillingCodeId");
            CreateIndex("dbo.Physician_CPTRates", "PhysicianId");
            CreateIndex("dbo.Physician_CPTRates", "BillingCodeId");
            AddForeignKey("dbo.Liaison_CPTRates", "BillingCodeId", "dbo.BillingCodes", "Id");
            AddForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons", "Id");
            AddForeignKey("dbo.Physician_CPTRates", "BillingCodeId", "dbo.BillingCodes", "Id");
            AddForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians", "Id");
            DropColumn("dbo.Liaisons", "Liaison_CPTRates_Id");
            DropColumn("dbo.Physicians", "Physician_CPTRates_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Physicians", "Physician_CPTRates_Id", c => c.Int());
            AddColumn("dbo.Liaisons", "Liaison_CPTRates_Id", c => c.Int());
            DropForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians");
            DropForeignKey("dbo.Physician_CPTRatesBillingCycle", "BillingCycle_Id", "dbo.BillingCycles");
            DropForeignKey("dbo.Physician_CPTRatesBillingCycle", "Physician_CPTRates_Id", "dbo.Physician_CPTRates");
            DropForeignKey("dbo.Physician_CPTRates", "BillingCodeId", "dbo.BillingCodes");
            DropForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons");
            DropForeignKey("dbo.Liaison_CPTRatesBillingCycle", "BillingCycle_Id", "dbo.BillingCycles");
            DropForeignKey("dbo.Liaison_CPTRatesBillingCycle", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates");
            DropForeignKey("dbo.Liaison_CPTRates", "BillingCodeId", "dbo.BillingCodes");
            DropForeignKey("dbo.BillingCodesBillingCycles", "BillingCycle_Id", "dbo.BillingCycles");
            DropForeignKey("dbo.BillingCodesBillingCycles", "BillingCodes_Id", "dbo.BillingCodes");
            DropForeignKey("dbo.BillingCodes", "BillingCategoryId", "dbo.BillingCategories");
            DropForeignKey("dbo.Patients_BillingCategories", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Patients_BillingCategories", "BillingCategoryId", "dbo.BillingCategories");
            DropForeignKey("dbo.BillingCategories", "BillingPeriodsId", "dbo.BillingPeriods");
            DropIndex("dbo.Physician_CPTRatesBillingCycle", new[] { "BillingCycle_Id" });
            DropIndex("dbo.Physician_CPTRatesBillingCycle", new[] { "Physician_CPTRates_Id" });
            DropIndex("dbo.Liaison_CPTRatesBillingCycle", new[] { "BillingCycle_Id" });
            DropIndex("dbo.Liaison_CPTRatesBillingCycle", new[] { "Liaison_CPTRates_Id" });
            DropIndex("dbo.BillingCodesBillingCycles", new[] { "BillingCycle_Id" });
            DropIndex("dbo.BillingCodesBillingCycles", new[] { "BillingCodes_Id" });
            DropIndex("dbo.Physician_CPTRates", new[] { "BillingCodeId" });
            DropIndex("dbo.Physician_CPTRates", new[] { "PhysicianId" });
            DropIndex("dbo.Liaison_CPTRates", new[] { "BillingCodeId" });
            DropIndex("dbo.Liaison_CPTRates", new[] { "LiaisonId" });
            DropIndex("dbo.Patients_BillingCategories", new[] { "BillingCategoryId" });
            DropIndex("dbo.Patients_BillingCategories", new[] { "PatientId" });
            DropIndex("dbo.BillingCategories", new[] { "BillingPeriodsId" });
            DropIndex("dbo.BillingCodes", new[] { "BillingCategoryId" });
            AlterColumn("dbo.Physician_CPTRates", "PhysicianId", c => c.Int(nullable: false));
            AlterColumn("dbo.Liaison_CPTRates", "LiaisonId", c => c.Int(nullable: false));
            DropColumn("dbo.Physician_CPTRates", "BillingCodeId");
            DropColumn("dbo.Liaison_CPTRates", "BillingCodeId");
            DropTable("dbo.Physician_CPTRatesBillingCycle");
            DropTable("dbo.Liaison_CPTRatesBillingCycle");
            DropTable("dbo.BillingCodesBillingCycles");
            DropTable("dbo.Patients_BillingCategories");
            DropTable("dbo.BillingPeriods");
            DropTable("dbo.BillingCategories");
            DropTable("dbo.BillingCodes");
            CreateIndex("dbo.Physicians", "Physician_CPTRates_Id");
            CreateIndex("dbo.Liaisons", "Liaison_CPTRates_Id");
            AddForeignKey("dbo.Physicians", "Physician_CPTRates_Id", "dbo.Physician_CPTRates", "Id");
            AddForeignKey("dbo.Liaisons", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates", "Id");
        }
    }
}
