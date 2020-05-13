namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedTableRpm_DeviceMappingAttachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rpm_DeviceMappingAttachments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Image = c.String(),
                        DeviceMappingHistoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.DeviceMappingHistories", t => t.DeviceMappingHistoryId, cascadeDelete: true)
                .Index(t => t.DeviceMappingHistoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rpm_DeviceMappingAttachments", "DeviceMappingHistoryId", "dbo.DeviceMappingHistories");
            DropIndex("dbo.Rpm_DeviceMappingAttachments", new[] { "DeviceMappingHistoryId" });
            DropTable("dbo.Rpm_DeviceMappingAttachments");
        }
    }
}
