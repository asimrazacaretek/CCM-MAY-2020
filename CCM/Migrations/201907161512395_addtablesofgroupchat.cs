namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablesofgroupchat : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupChatDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupChatId = c.Int(nullable: false),
                        Message = c.String(),
                        CreatedOn = c.DateTime(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChatParticipants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupChatId = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChatName = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ChatStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GroupChats");
            DropTable("dbo.GroupChatParticipants");
            DropTable("dbo.GroupChatDetails");
        }
    }
}
