namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsingroupchats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupChats", "isTicket", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupChats", "TicketNum", c => c.String());
            AddColumn("dbo.GroupChats", "TicketTitle", c => c.String());
            AddColumn("dbo.GroupChats", "Department", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupChats", "Department");
            DropColumn("dbo.GroupChats", "TicketTitle");
            DropColumn("dbo.GroupChats", "TicketNum");
            DropColumn("dbo.GroupChats", "isTicket");
        }
    }
}
