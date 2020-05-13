namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedRelationDevaicesBrandModelId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Device_BrandModelId", c => c.Int());
            CreateIndex("dbo.Devices", "Device_BrandModelId");
            AddForeignKey("dbo.Devices", "Device_BrandModelId", "dbo.Device_BrandModel", "Id");
            DropColumn("dbo.Devices", "ModelNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Devices", "ModelNumber", c => c.String());
            DropForeignKey("dbo.Devices", "Device_BrandModelId", "dbo.Device_BrandModel");
            DropIndex("dbo.Devices", new[] { "Device_BrandModelId" });
            DropColumn("dbo.Devices", "Device_BrandModelId");
        }
    }
}
