namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnnamecreatedbyingroupchatparticipents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupChatParticipants", "UserId", c => c.String());
            DropColumn("dbo.GroupChatParticipants", "CreatedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GroupChatParticipants", "CreatedBy", c => c.String());
            DropColumn("dbo.GroupChatParticipants", "UserId");
        }
    }
}
