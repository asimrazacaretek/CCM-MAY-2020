namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_columnName_CCMCycleStatuss : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "clinicalSignOffQueCounter", c => c.Int());
            DropColumn("dbo.CCMCycleStatus", "clinicalSignOffQue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CCMCycleStatus", "clinicalSignOffQue", c => c.Int());
            DropColumn("dbo.CCMCycleStatus", "clinicalSignOffQueCounter");
        }
    }
}
