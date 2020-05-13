namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnenrolleridinpatientappintmentanddoctorschedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorSchedules", "EnrollerID", c => c.Int());
            AddColumn("dbo.PatientAppointments", "EnrollerID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientAppointments", "EnrollerID");
            DropColumn("dbo.DoctorSchedules", "EnrollerID");
        }
    }
}
