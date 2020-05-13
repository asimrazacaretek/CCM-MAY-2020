namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedRelationDevaicesVderdorId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "VendorId", c => c.Int());
            CreateIndex("dbo.Devices", "VendorId");
            AddForeignKey("dbo.Devices", "VendorId", "dbo.Device_Vendor", "Id");
            DropColumn("dbo.Devices", "VendorName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Devices", "VendorName", c => c.String());
            DropForeignKey("dbo.Devices", "VendorId", "dbo.Device_Vendor");
            DropIndex("dbo.Devices", new[] { "VendorId" });
            DropColumn("dbo.Devices", "VendorId");
        }
    }
}
