namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedRelationDevaicesBrandId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Devices_BrandId", c => c.Int());
            CreateIndex("dbo.Devices", "Devices_BrandId");
            AddForeignKey("dbo.Devices", "Devices_BrandId", "dbo.Devices_Brand", "Id");
            DropColumn("dbo.Devices", "DeviceName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Devices", "DeviceName", c => c.String());
            DropForeignKey("dbo.Devices", "Devices_BrandId", "dbo.Devices_Brand");
            DropIndex("dbo.Devices", new[] { "Devices_BrandId" });
            DropColumn("dbo.Devices", "Devices_BrandId");
        }
    }
}
