namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewTablesGroupsChatsParticipetsMessages : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GroupChats", newName: "GroupChats");
            CreateTable(
                "dbo.GroupChatMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupChatDetailsId = c.Int(nullable: false),
                        MessageId = c.Int(nullable: false),
                        ReceivedBy = c.String(),
                        ReceivedOn = c.DateTime(nullable: false),
                        isNew = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChatName = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        GroupStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChatsDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupChatId = c.Int(nullable: false),
                        Message = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChatsParticipants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupChatId = c.Int(nullable: false),
                        UserId = c.String(),
                        isCreater = c.Boolean(nullable: false),
                        Notify = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GroupChatsParticipants");
            DropTable("dbo.GroupChatsDetails");
            DropTable("dbo.GroupChats");
            DropTable("dbo.GroupChatMessages");
            //RenameTable(name: "dbo.GroupChats1", newName: "GroupChats");
        }
    }
}
