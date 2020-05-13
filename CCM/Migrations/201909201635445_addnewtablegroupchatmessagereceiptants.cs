namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablegroupchatmessagereceiptants : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupChatNewMessages",
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
            
            DropColumn("dbo.GroupChatDetails", "isNew");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GroupChatDetails", "isNew", c => c.Boolean(nullable: false));
            DropTable("dbo.GroupChatNewMessages");
        }
    }
}
