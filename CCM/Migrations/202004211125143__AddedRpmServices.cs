namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddedRpmServices : DbMigration
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
                        Devices_BrandId = c.Int(),
                        RPMServiceId = c.Int(),
                        Device_BrandModelId = c.Int(),
                        SerialNumber = c.String(),
                        DatePurchase = c.DateTime(),
                        VendorId = c.Int(),
                        DeviceStatusId = c.Int(nullable: false),
                        IsActive = c.Int(nullable: false),
                        ReasonForDeactivate = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Device_BrandModel", t => t.Device_BrandModelId)
                .ForeignKey("dbo.Device_Vendor", t => t.VendorId)
                .ForeignKey("dbo.Devices_Brand", t => t.Devices_BrandId)
                .ForeignKey("dbo.RPMServices", t => t.RPMServiceId)
                .Index(t => t.Devices_BrandId)
                .Index(t => t.RPMServiceId)
                .Index(t => t.Device_BrandModelId)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.Device_BrandModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Devices_BrandId = c.Int(),
                        Device_VendorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Device_Vendor", t => t.Device_VendorId)
                .ForeignKey("dbo.Devices_Brand", t => t.Devices_BrandId)
                .Index(t => t.Devices_BrandId)
                .Index(t => t.Device_VendorId);
            
            CreateTable(
                "dbo.Device_Vendor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Address = c.String(),
                        PhoneNumber = c.String(),
                        IsActive = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Devices_Brand",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            DropForeignKey("dbo.Devices", "Devices_BrandId", "dbo.Devices_Brand");
            DropForeignKey("dbo.Devices", "VendorId", "dbo.Device_Vendor");
            DropForeignKey("dbo.Devices", "Device_BrandModelId", "dbo.Device_BrandModel");
            DropForeignKey("dbo.Device_BrandModel", "Devices_BrandId", "dbo.Devices_Brand");
            DropForeignKey("dbo.Device_BrandModel", "Device_VendorId", "dbo.Device_Vendor");
            DropIndex("dbo.Device_BrandModel", new[] { "Device_VendorId" });
            DropIndex("dbo.Device_BrandModel", new[] { "Devices_BrandId" });
            DropIndex("dbo.Devices", new[] { "VendorId" });
            DropIndex("dbo.Devices", new[] { "Device_BrandModelId" });
            DropIndex("dbo.Devices", new[] { "RPMServiceId" });
            DropIndex("dbo.Devices", new[] { "Devices_BrandId" });
            DropIndex("dbo.Patients_Services", new[] { "RPMServiceId" });
            DropIndex("dbo.Patients_Services", new[] { "PatientId" });
            DropIndex("dbo.Patients_Services", new[] { "DeviceId" });
            DropTable("dbo.RPMServices");
            DropTable("dbo.Devices_Brand");
            DropTable("dbo.Device_Vendor");
            DropTable("dbo.Device_BrandModel");
            DropTable("dbo.Devices");
            DropTable("dbo.Patients_Services");
        }
    }
}
