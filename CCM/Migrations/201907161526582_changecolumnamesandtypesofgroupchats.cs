namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnamesandtypesofgroupchats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupChatDetails", "CreatedBy", c => c.String());
            AddColumn("dbo.GroupChatParticipants", "CreatedBy", c => c.String());
            AddColumn("dbo.GroupChats", "UpdatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.GroupChats", "UpdatedBy", c => c.String());
            AlterColumn("dbo.GroupChats", "CreatedBy", c => c.String());
            DropColumn("dbo.GroupChatDetails", "UserId");
            DropColumn("dbo.GroupChatParticipants", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GroupChatParticipants", "UserID", c => c.Int(nullable: false));
            AddColumn("dbo.GroupChatDetails", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.GroupChats", "CreatedBy", c => c.Int(nullable: false));
            DropColumn("dbo.GroupChats", "UpdatedBy");
            DropColumn("dbo.GroupChats", "UpdatedOn");
            DropColumn("dbo.GroupChatParticipants", "CreatedBy");
            DropColumn("dbo.GroupChatDetails", "CreatedBy");
        }
    }
}
