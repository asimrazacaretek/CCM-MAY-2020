namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newcolumnsinccmcyclestatusapprovedbyrejectedby : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "ApprovedBy", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.CCMCycleStatus", "RejectedBy", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "RejectedBy");
            DropColumn("dbo.CCMCycleStatus", "ApprovedBy");
        }
    }
}
