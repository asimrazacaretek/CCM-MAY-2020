namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnnameandaddrelationintables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorSchedules", "SaleStaffID", c => c.Int());
            AddColumn("dbo.DoctorTimings", "SaleStaffID", c => c.Int());
            AddColumn("dbo.PatientAppointments", "SaleStaffID", c => c.Int());
            AddColumn("dbo.PhysicianHolidays", "SaleStaffID", c => c.Int());
            CreateIndex("dbo.DoctorSchedules", "SaleStaffID");
            CreateIndex("dbo.DoctorTimings", "SaleStaffID");
            CreateIndex("dbo.PatientAppointments", "SaleStaffID");
            CreateIndex("dbo.PhysicianHolidays", "SaleStaffID");
            AddForeignKey("dbo.DoctorSchedules", "SaleStaffID", "dbo.SaleStaffs", "Id");
            AddForeignKey("dbo.DoctorTimings", "SaleStaffID", "dbo.SaleStaffs", "Id");
            AddForeignKey("dbo.PatientAppointments", "SaleStaffID", "dbo.SaleStaffs", "Id");
            AddForeignKey("dbo.PhysicianHolidays", "SaleStaffID", "dbo.SaleStaffs", "Id");
            DropColumn("dbo.DoctorSchedules", "EnrollerID");
            DropColumn("dbo.DoctorTimings", "EnrollerID");
            DropColumn("dbo.PatientAppointments", "EnrollerID");
            DropColumn("dbo.PhysicianHolidays", "EnrollerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PhysicianHolidays", "EnrollerID", c => c.Int());
            AddColumn("dbo.PatientAppointments", "EnrollerID", c => c.Int());
            AddColumn("dbo.DoctorTimings", "EnrollerID", c => c.Int());
            AddColumn("dbo.DoctorSchedules", "EnrollerID", c => c.Int());
            DropForeignKey("dbo.PhysicianHolidays", "SaleStaffID", "dbo.SaleStaffs");
            DropForeignKey("dbo.PatientAppointments", "SaleStaffID", "dbo.SaleStaffs");
            DropForeignKey("dbo.DoctorTimings", "SaleStaffID", "dbo.SaleStaffs");
            DropForeignKey("dbo.DoctorSchedules", "SaleStaffID", "dbo.SaleStaffs");
            DropIndex("dbo.PhysicianHolidays", new[] { "SaleStaffID" });
            DropIndex("dbo.PatientAppointments", new[] { "SaleStaffID" });
            DropIndex("dbo.DoctorTimings", new[] { "SaleStaffID" });
            DropIndex("dbo.DoctorSchedules", new[] { "SaleStaffID" });
            DropColumn("dbo.PhysicianHolidays", "SaleStaffID");
            DropColumn("dbo.PatientAppointments", "SaleStaffID");
            DropColumn("dbo.DoctorTimings", "SaleStaffID");
            DropColumn("dbo.DoctorSchedules", "SaleStaffID");
        }
    }
}
