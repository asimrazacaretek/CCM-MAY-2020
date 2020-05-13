namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnlogouttimetologinhistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoginHistories", "LogOutDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoginHistories", "LogOutDateTime");
        }
    }
}
