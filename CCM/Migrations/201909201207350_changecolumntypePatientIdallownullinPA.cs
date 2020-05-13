namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumntypePatientIdallownullinPA : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PatientAppointments", "PatientID", "dbo.Patients");
            DropIndex("dbo.PatientAppointments", new[] { "PatientID" });
            AlterColumn("dbo.PatientAppointments", "PatientID", c => c.Int());
            CreateIndex("dbo.PatientAppointments", "PatientID");
            AddForeignKey("dbo.PatientAppointments", "PatientID", "dbo.Patients", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientAppointments", "PatientID", "dbo.Patients");
            DropIndex("dbo.PatientAppointments", new[] { "PatientID" });
            AlterColumn("dbo.PatientAppointments", "PatientID", c => c.Int(nullable: false));
            CreateIndex("dbo.PatientAppointments", "PatientID");
            AddForeignKey("dbo.PatientAppointments", "PatientID", "dbo.Patients", "Id", cascadeDelete: true);
        }
    }
}
