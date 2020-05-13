namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsDoctorTypeandIsCCMProviderandEMRnumandEmrType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "EMRNumber", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Patients", "EMRType", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.SecondaryDoctors", "DoctorType", c => c.String());
            AddColumn("dbo.SecondaryDoctors", "isCCMProvider", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecondaryDoctors", "isCCMProvider");
            DropColumn("dbo.SecondaryDoctors", "DoctorType");
            DropColumn("dbo.Patients", "EMRType");
            DropColumn("dbo.Patients", "EMRNumber");
        }
    }
}
