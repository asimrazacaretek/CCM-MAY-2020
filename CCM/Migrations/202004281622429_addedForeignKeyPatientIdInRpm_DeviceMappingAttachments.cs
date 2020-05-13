namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedForeignKeyPatientIdInRpm_DeviceMappingAttachments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rpm_DeviceMappingAttachments", "PatientId", c => c.Int());
            CreateIndex("dbo.Rpm_DeviceMappingAttachments", "PatientId");
            AddForeignKey("dbo.Rpm_DeviceMappingAttachments", "PatientId", "dbo.Patients", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rpm_DeviceMappingAttachments", "PatientId", "dbo.Patients");
            DropIndex("dbo.Rpm_DeviceMappingAttachments", new[] { "PatientId" });
            DropColumn("dbo.Rpm_DeviceMappingAttachments", "PatientId");
        }
    }
}
