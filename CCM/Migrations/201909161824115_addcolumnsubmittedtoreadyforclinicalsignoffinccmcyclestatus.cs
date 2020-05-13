namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsubmittedtoreadyforclinicalsignoffinccmcyclestatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "SubmittedToReadyforClinicalSignoff", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "SubmittedToReadyforClinicalSignoff");
        }
    }
}
