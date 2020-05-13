namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnnotestoccmcyclestatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "CCMNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "CCMNotes");
        }
    }
}
