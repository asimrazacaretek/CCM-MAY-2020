namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddedTablesDevicesModels : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Device_BrandModel", "Devices_BrandId", "dbo.Devices_Brand");
            DropForeignKey("dbo.Device_BrandModel", "Device_VendorId", "dbo.Device_Vendor");
            DropIndex("dbo.Device_BrandModel", new[] { "Device_VendorId" });
            DropIndex("dbo.Device_BrandModel", new[] { "Devices_BrandId" });
            DropTable("dbo.Devices_Brand");
            DropTable("dbo.Device_Vendor");
            DropTable("dbo.Device_BrandModel");
        }
    }
}
