namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddASSIGNEE_TICKET : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssigneeTickets",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ticketSubject = c.String(),
                        ticketType = c.String(),
                        ticketPriority = c.String(),
                        StatusId = c.Int(),
                        ticketTat = c.String(),
                        CreatorNotes = c.String(),
                        createdBy = c.String(),
                        inProgressCreatedDate = c.DateTime(),
                        inProgressWeekDay = c.String(),
                        inProgressWaitTime = c.Double(nullable: false),
                        closesCreatedDate = c.DateTime(),
                        closesWeekDay = c.String(),
                        closeResoloutionTime = c.Double(nullable: false),
                        UserTicketGenerationId = c.Int(),
                        AssigneeNotes = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.UserTicketGenerations", t => t.UserTicketGenerationId)
                .Index(t => t.StatusId)
                .Index(t => t.UserTicketGenerationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssigneeTickets", "UserTicketGenerationId", "dbo.UserTicketGenerations");
            DropForeignKey("dbo.AssigneeTickets", "StatusId", "dbo.Status");
            DropIndex("dbo.AssigneeTickets", new[] { "UserTicketGenerationId" });
            DropIndex("dbo.AssigneeTickets", new[] { "StatusId" });
            DropTable("dbo.AssigneeTickets");
        }
    }
}
