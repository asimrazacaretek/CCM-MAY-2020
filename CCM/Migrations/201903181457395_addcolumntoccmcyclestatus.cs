namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumntoccmcyclestatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "CcmReconciliationDate", c => c.DateTime());
            AddColumn("dbo.CCMCycleStatus", "CcmClaimSubmissionDate", c => c.DateTime());
            AddColumn("dbo.CCMCycleStatus", "CcmClinicalSignOffDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "CcmClinicalSignOffDate");
            DropColumn("dbo.CCMCycleStatus", "CcmClaimSubmissionDate");
            DropColumn("dbo.CCMCycleStatus", "CcmReconciliationDate");
        }
    }
}
