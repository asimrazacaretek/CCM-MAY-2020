namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumncycletoreviewtimeccm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReviewTimeCcms", "Cycle", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReviewTimeCcms", "Cycle");
        }
    }
}
