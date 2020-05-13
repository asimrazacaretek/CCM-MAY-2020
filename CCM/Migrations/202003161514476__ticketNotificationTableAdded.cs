namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _ticketNotificationTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketNotificationHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserTicketId = c.Int(),
                        TicketGenerationId = c.Int(),
                        TypeId = c.Int(),
                        PriorityId = c.Int(),
                        StatusId = c.Int(),
                        creatorNotes = c.String(),
                        UserId = c.String(),
                        PatientId = c.Int(),
                        createdBy = c.String(),
                        createdDate = c.DateTime(),
                        createdWeekDay = c.String(),
                        notify = c.Boolean(nullable: false),
                        ticketHonorId = c.String(),
                        isTicketHonor = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .ForeignKey("dbo.Priorities", t => t.PriorityId)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.TicketGenerations", t => t.TicketGenerationId)
                .ForeignKey("dbo.Types", t => t.TypeId)
                .Index(t => t.TicketGenerationId)
                .Index(t => t.TypeId)
                .Index(t => t.PriorityId)
                .Index(t => t.StatusId)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketNotificationHistories", "TypeId", "dbo.Types");
            DropForeignKey("dbo.TicketNotificationHistories", "TicketGenerationId", "dbo.TicketGenerations");
            DropForeignKey("dbo.TicketNotificationHistories", "StatusId", "dbo.Status");
            DropForeignKey("dbo.TicketNotificationHistories", "PriorityId", "dbo.Priorities");
            DropForeignKey("dbo.TicketNotificationHistories", "PatientId", "dbo.Patients");
            DropIndex("dbo.TicketNotificationHistories", new[] { "PatientId" });
            DropIndex("dbo.TicketNotificationHistories", new[] { "StatusId" });
            DropIndex("dbo.TicketNotificationHistories", new[] { "PriorityId" });
            DropIndex("dbo.TicketNotificationHistories", new[] { "TypeId" });
            DropIndex("dbo.TicketNotificationHistories", new[] { "TicketGenerationId" });
            DropTable("dbo.TicketNotificationHistories");
        }
    }
}
