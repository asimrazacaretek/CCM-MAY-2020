namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumntype1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DoctorSchedules", "UpdatedDate", c => c.DateTime());
            AlterColumn("dbo.PatientAppointments", "UpdatedBy", c => c.Int());
            AlterColumn("dbo.PatientAppointments", "UpdateOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientAppointments", "UpdateOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PatientAppointments", "UpdatedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.DoctorSchedules", "UpdatedDate", c => c.DateTime(nullable: false));
        }
    }
}
