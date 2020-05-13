namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnrejecttiondatetoccmcyclestatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "CcmRejectedDate", c => c.DateTime());
            AddColumn("dbo.CCMCycleStatus", "RejectedCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "RejectedCount");
            DropColumn("dbo.CCMCycleStatus", "CcmRejectedDate");
        }
    }
}
