namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnsize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientProfile_Contact", "CellPhoneNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "CellPhoneNumber2", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "HomePhoneNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "HomePhoneNumber2", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber2", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "EmergencyNumber", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "EmergencyNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_Contact", "EmergencyNumber2", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryMobilePhoneNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryMobilePhoneNumber2", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryHomePhoneNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryHomePhoneNumber2", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryMobilePhoneNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryMobilePhoneNumber2", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryHomePhoneNumber1", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryHomePhoneNumber2", c => c.String(maxLength: 15, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryHomePhoneNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryHomePhoneNumber1", c => c.String());
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryMobilePhoneNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_UrgencyContact", "SecondaryMobilePhoneNumber1", c => c.String());
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryHomePhoneNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryHomePhoneNumber1", c => c.String());
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryMobilePhoneNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_UrgencyContact", "PrimaryMobilePhoneNumber1", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "EmergencyNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "EmergencyNumber1", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "EmergencyNumber", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "WorkPhoneNumber1", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "HomePhoneNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "HomePhoneNumber1", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "CellPhoneNumber2", c => c.String());
            AlterColumn("dbo.PatientProfile_Contact", "CellPhoneNumber1", c => c.String());
        }
    }
}
