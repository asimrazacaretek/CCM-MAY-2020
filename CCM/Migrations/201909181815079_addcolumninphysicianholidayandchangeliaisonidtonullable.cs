namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumninphysicianholidayandchangeliaisonidtonullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DoctorTimings", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.DoctorSchedules", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.PatientAppointments", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.PhysicianHolidays", "LiaisonID", "dbo.Liaisons");
            DropIndex("dbo.DoctorSchedules", new[] { "LiaisonID" });
            DropIndex("dbo.DoctorTimings", new[] { "LiaisonID" });
            DropIndex("dbo.PatientAppointments", new[] { "LiaisonID" });
            DropIndex("dbo.PhysicianHolidays", new[] { "LiaisonID" });
            AddColumn("dbo.PhysicianHolidays", "EnrollerID", c => c.Int());
            AlterColumn("dbo.DoctorSchedules", "LiaisonID", c => c.Int());
            AlterColumn("dbo.PatientAppointments", "LiaisonID", c => c.Int());
            AlterColumn("dbo.PhysicianHolidays", "LiaisonID", c => c.Int());
            CreateIndex("dbo.DoctorSchedules", "LiaisonID");
            CreateIndex("dbo.PatientAppointments", "LiaisonID");
            CreateIndex("dbo.PhysicianHolidays", "LiaisonID");
            AddForeignKey("dbo.DoctorSchedules", "LiaisonID", "dbo.Liaisons", "Id");
            AddForeignKey("dbo.PatientAppointments", "LiaisonID", "dbo.Liaisons", "Id");
            AddForeignKey("dbo.PhysicianHolidays", "LiaisonID", "dbo.Liaisons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhysicianHolidays", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.PatientAppointments", "LiaisonID", "dbo.Liaisons");
            DropForeignKey("dbo.DoctorSchedules", "LiaisonID", "dbo.Liaisons");
            DropIndex("dbo.PhysicianHolidays", new[] { "LiaisonID" });
            DropIndex("dbo.PatientAppointments", new[] { "LiaisonID" });
            DropIndex("dbo.DoctorSchedules", new[] { "LiaisonID" });
            AlterColumn("dbo.PhysicianHolidays", "LiaisonID", c => c.Int(nullable: false));
            AlterColumn("dbo.PatientAppointments", "LiaisonID", c => c.Int(nullable: false));
            AlterColumn("dbo.DoctorSchedules", "LiaisonID", c => c.Int(nullable: false));
            DropColumn("dbo.PhysicianHolidays", "EnrollerID");
            CreateIndex("dbo.PhysicianHolidays", "LiaisonID");
            CreateIndex("dbo.PatientAppointments", "LiaisonID");
            CreateIndex("dbo.DoctorTimings", "LiaisonID");
            CreateIndex("dbo.DoctorSchedules", "LiaisonID");
            AddForeignKey("dbo.PhysicianHolidays", "LiaisonID", "dbo.Liaisons", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PatientAppointments", "LiaisonID", "dbo.Liaisons", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DoctorSchedules", "LiaisonID", "dbo.Liaisons", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DoctorTimings", "LiaisonID", "dbo.Liaisons", "Id");
        }
    }
}
