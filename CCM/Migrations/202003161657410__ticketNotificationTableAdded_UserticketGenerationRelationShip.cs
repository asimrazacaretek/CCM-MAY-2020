namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _ticketNotificationTableAdded_UserticketGenerationRelationShip : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketNotificationHistories", "UserTicketGenerationId", c => c.Int());
            CreateIndex("dbo.TicketNotificationHistories", "UserTicketGenerationId");
            AddForeignKey("dbo.TicketNotificationHistories", "UserTicketGenerationId", "dbo.UserTicketGenerations", "Id");
            DropColumn("dbo.TicketNotificationHistories", "UserTicketId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketNotificationHistories", "UserTicketId", c => c.Int());
            DropForeignKey("dbo.TicketNotificationHistories", "UserTicketGenerationId", "dbo.UserTicketGenerations");
            DropIndex("dbo.TicketNotificationHistories", new[] { "UserTicketGenerationId" });
            DropColumn("dbo.TicketNotificationHistories", "UserTicketGenerationId");
        }
    }
}
