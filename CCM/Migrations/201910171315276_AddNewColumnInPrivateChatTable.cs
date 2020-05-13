namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewColumnInPrivateChatTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrivateChats", "SenderName", c => c.String());
            AddColumn("dbo.PrivateChats", "ReceiverName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrivateChats", "ReceiverName");
            DropColumn("dbo.PrivateChats", "SenderName");
        }
    }
}
