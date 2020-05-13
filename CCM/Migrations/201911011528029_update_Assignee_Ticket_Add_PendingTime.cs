namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_Assignee_Ticket_Add_PendingTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssigneeTickets", "pendingCreatedDate", c => c.DateTime());
            AddColumn("dbo.AssigneeTickets", "pendingWeekDay", c => c.String());
            AddColumn("dbo.AssigneeTickets", "pendingTime", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssigneeTickets", "pendingTime");
            DropColumn("dbo.AssigneeTickets", "pendingWeekDay");
            DropColumn("dbo.AssigneeTickets", "pendingCreatedDate");
        }
    }
}
