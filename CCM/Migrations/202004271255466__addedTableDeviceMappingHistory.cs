namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedTableDeviceMappingHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeviceMappingHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Type = c.String(),
                        DatePerformed = c.DateTime(nullable: false),
                        RPMServiceId = c.Int(),
                        PatientId = c.Int(),
                        DevicetId = c.Int(),
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
            DropForeignKey("dbo.DeviceMappingHistories", "RPMServiceId", "dbo.RPMServices");
            DropForeignKey("dbo.DeviceMappingHistories", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.DeviceMappingHistories", "DevicetId", "dbo.Devices");
            DropIndex("dbo.DeviceMappingHistories", new[] { "DevicetId" });
            DropIndex("dbo.DeviceMappingHistories", new[] { "PatientId" });
            DropIndex("dbo.DeviceMappingHistories", new[] { "RPMServiceId" });
            DropTable("dbo.DeviceMappingHistories");
        }
    }
}
