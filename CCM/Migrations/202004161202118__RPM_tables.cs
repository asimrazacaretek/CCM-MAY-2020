namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _RPM_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients_Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.Int(),
                        PatientId = c.Int(),
                        RPMServiceId = c.Int(),
                        IsAssigned = c.Int(nullable: false),
                        CommentsOnAssigement = c.String(),
                        AssignedDate = c.DateTime(),
                        IsActive = c.Int(nullable: false),
                        ReasonForDeactivate = c.String(),
                        CreatedOn = c.DateTime(),
                        UpdatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RPMServices", t => t.RPMServiceId)
                .ForeignKey("dbo.Devices", t => t.DeviceId)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .Index(t => t.DeviceId)
                .Index(t => t.PatientId)
                .Index(t => t.RPMServiceId);
            
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceName = c.String(),
                        RPMServiceId = c.Int(),
                        ModelNumber = c.String(),
                        SerialNumber = c.String(),
                        DatePurchase = c.DateTime(),
                        VendorName = c.String(),
                        DeviceStatusId = c.Int(nullable: false),
                        IsActive = c.Int(nullable: false),
                        ReasonForDeactivate = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RPMServices", t => t.RPMServiceId)
                .Index(t => t.RPMServiceId);
            
            CreateTable(
                "dbo.RPMServices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceName = c.String(),
                        IsActive = c.Int(nullable: false),
                        ReasonForDeactivate = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients_Services", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Patients_Services", "DeviceId", "dbo.Devices");
            DropForeignKey("dbo.Devices", "RPMServiceId", "dbo.RPMServices");
            DropForeignKey("dbo.Patients_Services", "RPMServiceId", "dbo.RPMServices");
            DropIndex("dbo.Devices", new[] { "RPMServiceId" });
            DropIndex("dbo.Patients_Services", new[] { "RPMServiceId" });
            DropIndex("dbo.Patients_Services", new[] { "PatientId" });
            DropIndex("dbo.Patients_Services", new[] { "DeviceId" });
            DropTable("dbo.RPMServices");
            DropTable("dbo.Devices");
            DropTable("dbo.Patients_Services");
        }
    }
}
