namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddedColumnIsSyncFormliveInDeviceMappingHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceMappingHistories", "IsSyncFormlive", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceMappingHistories", "IsSyncFormlive");
        }
    }
}
