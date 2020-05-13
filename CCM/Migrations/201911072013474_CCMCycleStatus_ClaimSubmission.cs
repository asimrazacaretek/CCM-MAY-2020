namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CCMCycleStatus_ClaimSubmission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "claimSubmissionCounter", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "claimSubmissionCounter");
        }
    }
}
