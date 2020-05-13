namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnTwilioCallerIDinTextHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TextHistories", "TwilioCallerId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TextHistories", "TwilioCallerId");
        }
    }
}
