namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsCapitatedfromtoandhmoandorderby : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EnrollmentStatus", "OrderBy", c => c.Int(nullable: false));
            AddColumn("dbo.EnrollmentSubStatus", "OrderBy", c => c.Int(nullable: false));
            AddColumn("dbo.Patients", "CapitatedPatient", c => c.String(maxLength: 5, unicode: false));
            AddColumn("dbo.Patients", "CapitatedFrom", c => c.DateTime());
            AddColumn("dbo.Patients", "CapitatedTo", c => c.DateTime());
            AddColumn("dbo.PatientProfile_Insurance", "HMOInsuranceName", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.PatientProfile_Insurance", "HMOInsuranceID", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientProfile_Insurance", "HMOInsuranceID");
            DropColumn("dbo.PatientProfile_Insurance", "HMOInsuranceName");
            DropColumn("dbo.Patients", "CapitatedTo");
            DropColumn("dbo.Patients", "CapitatedFrom");
            DropColumn("dbo.Patients", "CapitatedPatient");
            DropColumn("dbo.EnrollmentSubStatus", "OrderBy");
            DropColumn("dbo.EnrollmentStatus", "OrderBy");
        }
    }
}
