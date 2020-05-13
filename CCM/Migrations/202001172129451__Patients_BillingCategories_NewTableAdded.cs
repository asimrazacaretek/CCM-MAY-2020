namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _Patients_BillingCategories_NewTableAdded : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients_BillingCategories", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Patients_BillingCategories", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.Patients_BillingCategories", new[] { "BillingCategoryId" });
            DropIndex("dbo.Patients_BillingCategories", new[] { "PatientId" });
            DropTable("dbo.Patients_BillingCategories");
        }
    }
}
