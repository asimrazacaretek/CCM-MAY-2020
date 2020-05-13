namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablespatienthospitalanddepartmentandhospitalsnew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hospitals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HospitalName = c.String(maxLength: 250, unicode: false),
                        City = c.String(maxLength: 100, unicode: false),
                        Country = c.String(maxLength: 50, unicode: false),
                        State = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientProfile_HospitalDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        HospitalsId = c.Int(nullable: false),
                        HospitalName = c.String(maxLength: 250, unicode: false),
                        AdmitDate = c.DateTime(nullable: false),
                        DischargeDate = c.DateTime(nullable: false),
                        Rate = c.Int(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        Reason = c.String(maxLength: 250, unicode: false),
                        Department = c.String(maxLength: 250, unicode: false),
                        ICD10Codes = c.String(maxLength: 250, unicode: false),
                        Doctors = c.String(maxLength: 250, unicode: false),
                        PhysicianId = c.Int(),
                        StayType = c.String(maxLength: 50, unicode: false),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hospitals", t => t.HospitalsId, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Physicians", t => t.PhysicianId)
                .Index(t => t.PatientId)
                .Index(t => t.HospitalsId)
                .Index(t => t.PhysicianId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientProfile_HospitalDetails", "PhysicianId", "dbo.Physicians");
            DropForeignKey("dbo.PatientProfile_HospitalDetails", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.PatientProfile_HospitalDetails", "HospitalsId", "dbo.Hospitals");
            DropIndex("dbo.PatientProfile_HospitalDetails", new[] { "PhysicianId" });
            DropIndex("dbo.PatientProfile_HospitalDetails", new[] { "HospitalsId" });
            DropIndex("dbo.PatientProfile_HospitalDetails", new[] { "PatientId" });
            DropTable("dbo.PatientProfile_HospitalDetails");
            DropTable("dbo.Hospitals");
            DropTable("dbo.Departments");
        }
    }
}
