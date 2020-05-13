namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumninparticipents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupChatParticipants", "isCreater", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupChatParticipants", "isCreater");
        }
    }
}
