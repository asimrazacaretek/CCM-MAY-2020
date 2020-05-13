namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddedtableRpm_PatientDeviceReading : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rpm_PatientDeviceReading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Battery = c.Int(),
                        Blood_glucose_mgdl = c.Double(),
                        Reading_type = c.String(),
                        Reading_id = c.Int(),
                        Time_zone_offset = c.Double(),
                        Blood_glucose_mmol = c.Double(),
                        Device_model = c.String(),
                        Date_recorded = c.DateTime(),
                        Date_received = c.DateTime(),
                        Before_meal = c.Boolean(),
                        Device_id_RPMVendor = c.String(),
                        RPMServiceId = c.Int(),
                        PatientId = c.Int(),
                        DevicetId = c.Int(),
                        IsActive = c.Int(),
                        ReasonForDeactivate = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Devices", t => t.DevicetId)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .ForeignKey("dbo.RPMServices", t => t.RPMServiceId)
                .Index(t => t.RPMServiceId)
                .Index(t => t.PatientId)
                .Index(t => t.DevicetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rpm_PatientDeviceReading", "RPMServiceId", "dbo.RPMServices");
            DropForeignKey("dbo.Rpm_PatientDeviceReading", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Rpm_PatientDeviceReading", "DevicetId", "dbo.Devices");
            DropIndex("dbo.Rpm_PatientDeviceReading", new[] { "DevicetId" });
            DropIndex("dbo.Rpm_PatientDeviceReading", new[] { "PatientId" });
            DropIndex("dbo.Rpm_PatientDeviceReading", new[] { "RPMServiceId" });
            DropTable("dbo.Rpm_PatientDeviceReading");
        }
    }
}
