namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_columnName_CCMCycleStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "clinicalSignOffQue", c => c.Int());
            DropColumn("dbo.CCMCycleStatus", "claimSubmissionCounter");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CCMCycleStatus", "claimSubmissionCounter", c => c.Int());
            DropColumn("dbo.CCMCycleStatus", "clinicalSignOffQue");
        }
    }
}
