namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _Patients_BillingCategories_Relationship : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.BillingCodesBillingCycles", newName: "BillingCycleBillingCodes");
            DropPrimaryKey("dbo.BillingCycleBillingCodes");
            CreateTable(
                "dbo.BillingCategoryPatients",
                c => new
                    {
                        BillingCategory_BillingCategoryId = c.Int(nullable: false),
                        Patient_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BillingCategory_BillingCategoryId, t.Patient_Id })
                .ForeignKey("dbo.BillingCategories", t => t.BillingCategory_BillingCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.Patient_Id, cascadeDelete: true)
                .Index(t => t.BillingCategory_BillingCategoryId)
                .Index(t => t.Patient_Id);
            
            AddPrimaryKey("dbo.BillingCycleBillingCodes", new[] { "BillingCycle_Id", "BillingCodes_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillingCategoryPatients", "Patient_Id", "dbo.Patients");
            DropForeignKey("dbo.BillingCategoryPatients", "BillingCategory_BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.BillingCategoryPatients", new[] { "Patient_Id" });
            DropIndex("dbo.BillingCategoryPatients", new[] { "BillingCategory_BillingCategoryId" });
            DropPrimaryKey("dbo.BillingCycleBillingCodes");
            DropTable("dbo.BillingCategoryPatients");
            AddPrimaryKey("dbo.BillingCycleBillingCodes", new[] { "BillingCodes_Id", "BillingCycle_Id" });
            RenameTable(name: "dbo.BillingCycleBillingCodes", newName: "BillingCodesBillingCycles");
        }
    }
}
