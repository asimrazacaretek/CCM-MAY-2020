namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _CategoryStatusesTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoriesStatuses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(),
                        Status = c.String(maxLength: 50, unicode: false),
                        SubStatus = c.String(maxLength: 50, unicode: false),
                        Cycle = c.Int(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        ReconciliationDate = c.DateTime(),
                        ClaimSubmissionDate = c.DateTime(),
                        ClinicalSignOffDate = c.DateTime(),
                        RejectedDate = c.DateTime(),
                        RejectedCount = c.Int(nullable: false),
                        Notes = c.String(),
                        ApprovedBy = c.String(maxLength: 100, unicode: false),
                        RejectedBy = c.String(maxLength: 100, unicode: false),
                        SubmittedBy = c.String(maxLength: 100, unicode: false),
                        ReadyforClinicalSignOffDate = c.DateTime(),
                        SubmittedByReadyforClinicalSignoff = c.String(maxLength: 100, unicode: false),
                        IsRejectedByLiaison = c.Boolean(nullable: false),
                        RejectedbyLiaison = c.String(maxLength: 100, unicode: false),
                        RejectedDatebyLiaison = c.DateTime(),
                        SubmittedToReadyforClinicalSignoff = c.String(),
                        BillingCategoryName = c.String(),
                        BillingCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCategories", t => t.BillingCategoryId)
                .Index(t => t.BillingCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CategoriesStatuses", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.CategoriesStatuses", new[] { "BillingCategoryId" });
            DropTable("dbo.CategoriesStatuses");
        }
    }
}
