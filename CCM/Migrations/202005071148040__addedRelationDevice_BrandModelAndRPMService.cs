namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedRelationDevice_BrandModelAndRPMService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Device_BrandModel", "RPMServiceId", c => c.Int());
            CreateIndex("dbo.Device_BrandModel", "RPMServiceId");
            AddForeignKey("dbo.Device_BrandModel", "RPMServiceId", "dbo.RPMServices", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Device_BrandModel", "RPMServiceId", "dbo.RPMServices");
            DropIndex("dbo.Device_BrandModel", new[] { "RPMServiceId" });
            DropColumn("dbo.Device_BrandModel", "RPMServiceId");
        }
    }
}
