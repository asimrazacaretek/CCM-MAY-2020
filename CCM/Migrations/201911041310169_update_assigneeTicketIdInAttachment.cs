namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_assigneeTicketIdInAttachment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketAttachments", "assigneeTicketId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketAttachments", "assigneeTicketId");
        }
    }
}
