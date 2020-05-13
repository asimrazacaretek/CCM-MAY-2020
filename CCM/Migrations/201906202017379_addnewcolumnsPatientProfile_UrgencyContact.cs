namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnsPatientProfile_UrgencyContact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientProfile_UrgencyContact", "PrimaryEmail", c => c.String());
            AddColumn("dbo.PatientProfile_UrgencyContact", "PrimaryIsShareCarePlan", c => c.Boolean());
            AddColumn("dbo.PatientProfile_UrgencyContact", "SecondaryEmail", c => c.String());
            AddColumn("dbo.PatientProfile_UrgencyContact", "SecondaryIsShareCarePlan", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientProfile_UrgencyContact", "SecondaryIsShareCarePlan");
            DropColumn("dbo.PatientProfile_UrgencyContact", "SecondaryEmail");
            DropColumn("dbo.PatientProfile_UrgencyContact", "PrimaryIsShareCarePlan");
            DropColumn("dbo.PatientProfile_UrgencyContact", "PrimaryEmail");
        }
    }
}
