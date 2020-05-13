namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatientsProfileConcent_ImageAndNote_Relation_BillingCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientProfile_Consent", "Note", c => c.String());
            AddColumn("dbo.PatientProfile_Consent", "filePath", c => c.String());
            AddColumn("dbo.PatientProfile_Consent", "fileName", c => c.String());
            AddColumn("dbo.PatientProfile_Consent", "BillingCategoryId", c => c.Int());
            CreateIndex("dbo.PatientProfile_Consent", "BillingCategoryId");
            AddForeignKey("dbo.PatientProfile_Consent", "BillingCategoryId", "dbo.BillingCategories", "BillingCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientProfile_Consent", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.PatientProfile_Consent", new[] { "BillingCategoryId" });
            DropColumn("dbo.PatientProfile_Consent", "BillingCategoryId");
            DropColumn("dbo.PatientProfile_Consent", "fileName");
            DropColumn("dbo.PatientProfile_Consent", "filePath");
            DropColumn("dbo.PatientProfile_Consent", "Note");
        }
    }
}
