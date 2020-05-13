namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewTablePrivateChatAndonline : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OnlineUsers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Acitve = c.Boolean(nullable: false),
                        loginDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.PrivateChats",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        From = c.String(),
                        To = c.String(),
                        Message = c.String(),
                        DateSent = c.DateTime(nullable: false),
                        Read = c.Boolean(nullable: false),
                        Attachment = c.Boolean(nullable: false),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PrivateChats");
            DropTable("dbo.OnlineUsers");
        }
    }
}
