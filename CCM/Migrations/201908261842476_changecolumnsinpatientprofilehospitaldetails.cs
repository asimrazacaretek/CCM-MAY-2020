namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnsinpatientprofilehospitaldetails : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PatientProfile_HospitalDetails", "PhysicianId", "dbo.Physicians");
            DropIndex("dbo.PatientProfile_HospitalDetails", new[] { "PhysicianId" });
            AddColumn("dbo.PatientProfile_HospitalDetails", "City", c => c.String(maxLength: 250, unicode: false));
            AddColumn("dbo.PatientProfile_HospitalDetails", "Country", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.PatientProfile_HospitalDetails", "State", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.PatientProfile_HospitalDetails", "FullName", c => c.String(maxLength: 250, unicode: false));
            AddColumn("dbo.PatientProfile_HospitalDetails", "NPI", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.PatientProfile_HospitalDetails", "Phone", c => c.String(maxLength: 50, unicode: false));
            DropColumn("dbo.PatientProfile_HospitalDetails", "Doctors");
            DropColumn("dbo.PatientProfile_HospitalDetails", "PhysicianId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientProfile_HospitalDetails", "PhysicianId", c => c.Int());
            AddColumn("dbo.PatientProfile_HospitalDetails", "Doctors", c => c.String(maxLength: 250, unicode: false));
            DropColumn("dbo.PatientProfile_HospitalDetails", "Phone");
            DropColumn("dbo.PatientProfile_HospitalDetails", "NPI");
            DropColumn("dbo.PatientProfile_HospitalDetails", "FullName");
            DropColumn("dbo.PatientProfile_HospitalDetails", "State");
            DropColumn("dbo.PatientProfile_HospitalDetails", "Country");
            DropColumn("dbo.PatientProfile_HospitalDetails", "City");
            CreateIndex("dbo.PatientProfile_HospitalDetails", "PhysicianId");
            AddForeignKey("dbo.PatientProfile_HospitalDetails", "PhysicianId", "dbo.Physicians", "Id");
        }
    }
}
