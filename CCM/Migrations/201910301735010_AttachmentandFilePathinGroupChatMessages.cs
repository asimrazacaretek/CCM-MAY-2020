namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttachmentandFilePathinGroupChatMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupChatMessages", "Attachment", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupChatMessages", "FilePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupChatMessages", "FilePath");
            DropColumn("dbo.GroupChatMessages", "Attachment");
        }
    }
}
