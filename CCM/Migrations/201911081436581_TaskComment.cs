namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserTicketGenerationId = c.Int(),
                        noteText = c.String(),
                        createdDate = c.DateTime(nullable: false),
                        createdBy = c.String(),
                        AssigneeTicketId = c.Int(),
                        ticketActionName = c.String(),
                        updatedBy = c.String(),
                        updatedDate = c.DateTime(),
                        isDeleted = c.Boolean(nullable: false),
                        deletedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssigneeTickets", t => t.AssigneeTicketId)
                .ForeignKey("dbo.UserTicketGenerations", t => t.UserTicketGenerationId)
                .Index(t => t.UserTicketGenerationId)
                .Index(t => t.AssigneeTicketId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketComments", "UserTicketGenerationId", "dbo.UserTicketGenerations");
            DropForeignKey("dbo.TicketComments", "AssigneeTicketId", "dbo.AssigneeTickets");
            DropIndex("dbo.TicketComments", new[] { "AssigneeTicketId" });
            DropIndex("dbo.TicketComments", new[] { "UserTicketGenerationId" });
            DropTable("dbo.TicketComments");
        }
    }
}
