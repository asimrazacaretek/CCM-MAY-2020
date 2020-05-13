namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsinccmccyclestatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "CcmReadyforClinicalSignOffDate", c => c.DateTime());
            AddColumn("dbo.CCMCycleStatus", "SubmittedByReadyforClinicalSignoff", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.CCMCycleStatus", "IsRejectedByLiaison", c => c.Boolean(nullable: false));
            AddColumn("dbo.CCMCycleStatus", "RejectedbyLiaison", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.CCMCycleStatus", "CcmRejectedDatebyLiaison", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "CcmRejectedDatebyLiaison");
            DropColumn("dbo.CCMCycleStatus", "RejectedbyLiaison");
            DropColumn("dbo.CCMCycleStatus", "IsRejectedByLiaison");
            DropColumn("dbo.CCMCycleStatus", "SubmittedByReadyforClinicalSignoff");
            DropColumn("dbo.CCMCycleStatus", "CcmReadyforClinicalSignOffDate");
        }
    }
}
