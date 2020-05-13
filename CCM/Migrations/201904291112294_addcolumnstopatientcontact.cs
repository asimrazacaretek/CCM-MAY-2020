namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnstopatientcontact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "MobilePhoneNumber1", c => c.String());
            AddColumn("dbo.Patients", "MobilePhoneNumber2", c => c.String());
            AddColumn("dbo.Patients", "WorkPhoneNumber1", c => c.String());
            AddColumn("dbo.Patients", "WorkPhoneNumber2", c => c.String());
            AddColumn("dbo.Patients", "HomePhoneNumber1", c => c.String());
            AddColumn("dbo.Patients", "HomePhoneNumber2", c => c.String());
            AddColumn("dbo.Patients", "EmergencyNumber1", c => c.String());
            AddColumn("dbo.Patients", "EmergencyNumber2", c => c.String());
            AddColumn("dbo.Patients", "CaretakerPhoneNumber1", c => c.String());
            AddColumn("dbo.Patients", "CaretakerPhoneNumber2", c => c.String());
            AddColumn("dbo.PatientProfile_Contact", "CellPhoneNumber1", c => c.String());
            AddColumn("dbo.PatientProfile_Contact", "CellPhoneNumber2", c => c.String());
            AddColumn("dbo.PatientProfile_Contact", "HomePhoneNumber1", c => c.String());
            AddColumn("dbo.PatientProfile_Contact", "HomePhoneNumber2", c => c.String());
            AddColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber1", c => c.String());
            AddColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber2");
            DropColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber1");
            DropColumn("dbo.PatientProfile_Contact", "HomePhoneNumber2");
            DropColumn("dbo.PatientProfile_Contact", "HomePhoneNumber1");
            DropColumn("dbo.PatientProfile_Contact", "CellPhoneNumber2");
            DropColumn("dbo.PatientProfile_Contact", "CellPhoneNumber1");
            DropColumn("dbo.Patients", "CaretakerPhoneNumber2");
            DropColumn("dbo.Patients", "CaretakerPhoneNumber1");
            DropColumn("dbo.Patients", "EmergencyNumber2");
            DropColumn("dbo.Patients", "EmergencyNumber1");
            DropColumn("dbo.Patients", "HomePhoneNumber2");
            DropColumn("dbo.Patients", "HomePhoneNumber1");
            DropColumn("dbo.Patients", "WorkPhoneNumber2");
            DropColumn("dbo.Patients", "WorkPhoneNumber1");
            DropColumn("dbo.Patients", "MobilePhoneNumber2");
            DropColumn("dbo.Patients", "MobilePhoneNumber1");
        }
    }
}
