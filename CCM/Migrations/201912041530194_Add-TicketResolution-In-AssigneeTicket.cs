namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketResolutionInAssigneeTicket : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssigneeTickets", "TicketResolutionId", c => c.Int());
            CreateIndex("dbo.AssigneeTickets", "TicketResolutionId");
            AddForeignKey("dbo.AssigneeTickets", "TicketResolutionId", "dbo.TicketResolutions", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssigneeTickets", "TicketResolutionId", "dbo.TicketResolutions");
            DropIndex("dbo.AssigneeTickets", new[] { "TicketResolutionId" });
            DropColumn("dbo.AssigneeTickets", "TicketResolutionId");
        }
    }
}
