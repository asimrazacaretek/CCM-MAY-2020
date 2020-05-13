namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnactivitytoreveiwtimeccm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReviewTimeCcms", "Activity", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReviewTimeCcms", "Activity");
        }
    }
}
