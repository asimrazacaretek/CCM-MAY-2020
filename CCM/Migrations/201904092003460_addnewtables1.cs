namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtables1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorScheduleDeatils",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        ScheduleID = c.Int(nullable: false),
                        WeekDayName = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        DoctorSchedule_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DoctorSchedules", t => t.DoctorSchedule_ID)
                .Index(t => t.DoctorSchedule_ID);
            
            CreateTable(
                "dbo.DoctorSchedules",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LiaisonID = c.Int(nullable: false),
                        ClinicID = c.Int(nullable: false),
                        ScheduleValidTill = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        UpdatedBy = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        ScheduleValidFrom = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clinics", t => t.ClinicID, cascadeDelete: true)
                .ForeignKey("dbo.Liaisons", t => t.LiaisonID, cascadeDelete: true)
                .Index(t => t.LiaisonID)
                .Index(t => t.ClinicID);
            
            CreateTable(
                "dbo.Clinics",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        StateCode = c.String(),
                        Zip = c.String(),
                        Phone = c.String(),
                        Web = c.String(),
                        Email = c.String(),
                        Fax = c.String(),
                        Photo = c.String(),
                        NPI = c.String(),
                        IntegraTaxID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        PrimaryContactName = c.String(),
                        PrimaryContactNumber = c.String(),
                        SecondaryContactName = c.String(),
                        SecondaryContactNumber = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PatientAppointments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        LiaisonID = c.Int(nullable: false),
                        PatientID = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdateOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ClinicID = c.Int(),
                        AptStatus = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Liaisons", t => t.LiaisonID, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientID, cascadeDelete: true)
                .Index(t => t.LiaisonID)
                .Index(t => t.PatientID);
            
            CreateTable(
                "dbo.PhysicianHolidays",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LiaisonID = c.Int(nullable: false),
                        HDate = c.DateTime(),
                        ClinicID = c.Int(),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Liaisons", t => t.LiaisonID, cascadeDelete: true)
                .Index(t => t.LiaisonID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhysicianHolidays", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.PatientAppointments", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.PatientAppointments", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.DoctorSchedules", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.DoctorScheduleDeatils", "DoctorSchedule_ID", "dbo.DoctorSchedules");
            DropForeignKey("dbo.DoctorSchedules", "ClinicID", "dbo.Clinics");
            DropIndex("dbo.PhysicianHolidays", new[] { "LiaisonID" });
            DropIndex("dbo.PatientAppointments", new[] { "PatientID" });
            DropIndex("dbo.PatientAppointments", new[] { "LiaisonID" });
            DropIndex("dbo.DoctorSchedules", new[] { "ClinicID" });
            DropIndex("dbo.DoctorSchedules", new[] { "LiaisonID" });
            DropIndex("dbo.DoctorScheduleDeatils", new[] { "DoctorSchedule_ID" });
            DropTable("dbo.PhysicianHolidays");
            DropTable("dbo.PatientAppointments");
            DropTable("dbo.Clinics");
            DropTable("dbo.DoctorSchedules");
            DropTable("dbo.DoctorScheduleDeatils");
        }
    }
}
