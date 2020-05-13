namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedColumnCreatedByDeviceMappingHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceMappingHistories", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceMappingHistories", "CreatedBy");
        }
    }
}
