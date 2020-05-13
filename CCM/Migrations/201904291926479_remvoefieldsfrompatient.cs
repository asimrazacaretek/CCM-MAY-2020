namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remvoefieldsfrompatient : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Patients", "MobilePhoneNumber1");
            DropColumn("dbo.Patients", "MobilePhoneNumber2");
            DropColumn("dbo.Patients", "WorkPhoneNumber1");
            DropColumn("dbo.Patients", "WorkPhoneNumber2");
            DropColumn("dbo.Patients", "HomePhoneNumber1");
            DropColumn("dbo.Patients", "HomePhoneNumber2");
            DropColumn("dbo.Patients", "EmergencyNumber1");
            DropColumn("dbo.Patients", "EmergencyNumber2");
            DropColumn("dbo.Patients", "CaretakerPhoneNumber1");
            DropColumn("dbo.Patients", "CaretakerPhoneNumber2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Patients", "CaretakerPhoneNumber2", c => c.String());
            AddColumn("dbo.Patients", "CaretakerPhoneNumber1", c => c.String());
            AddColumn("dbo.Patients", "EmergencyNumber2", c => c.String());
            AddColumn("dbo.Patients", "EmergencyNumber1", c => c.String());
            AddColumn("dbo.Patients", "HomePhoneNumber2", c => c.String());
            AddColumn("dbo.Patients", "HomePhoneNumber1", c => c.String());
            AddColumn("dbo.Patients", "WorkPhoneNumber2", c => c.String());
            AddColumn("dbo.Patients", "WorkPhoneNumber1", c => c.String());
            AddColumn("dbo.Patients", "MobilePhoneNumber2", c => c.String());
            AddColumn("dbo.Patients", "MobilePhoneNumber1", c => c.String());
        }
    }
}
