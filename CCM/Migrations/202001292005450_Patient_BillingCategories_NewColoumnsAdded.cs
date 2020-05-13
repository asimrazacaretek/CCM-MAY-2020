namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patient_BillingCategories_NewColoumnsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients_BillingCategories", "DeEnrollmentReason", c => c.String());
            AddColumn("dbo.Patients_BillingCategories", "IsTranslator", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients_BillingCategories", "IsTranslator");
            DropColumn("dbo.Patients_BillingCategories", "DeEnrollmentReason");
        }
    }
}
