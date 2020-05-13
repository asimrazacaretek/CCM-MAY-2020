namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnstatusingroupchatdetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupChatDetails", "isNew", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupChatDetails", "isNew");
        }
    }
}
