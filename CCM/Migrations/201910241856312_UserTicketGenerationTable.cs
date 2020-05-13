namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTicketGenerationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTicketGenerations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketGenerationId = c.Int(),
                        TypeId = c.Int(),
                        PriorityId = c.Int(),
                        StatusId = c.Int(),
                        UserId = c.String(),
                        PatientId = c.Int(),
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
            DropForeignKey("dbo.UserTicketGenerations", "TypeId", "dbo.Types");
            DropForeignKey("dbo.UserTicketGenerations", "TicketGenerationId", "dbo.TicketGenerations");
            DropForeignKey("dbo.UserTicketGenerations", "StatusId", "dbo.Status");
            DropForeignKey("dbo.UserTicketGenerations", "PriorityId", "dbo.Priorities");
            DropForeignKey("dbo.UserTicketGenerations", "PatientId", "dbo.Patients");
            DropIndex("dbo.UserTicketGenerations", new[] { "PatientId" });
            DropIndex("dbo.UserTicketGenerations", new[] { "StatusId" });
            DropIndex("dbo.UserTicketGenerations", new[] { "PriorityId" });
            DropIndex("dbo.UserTicketGenerations", new[] { "TypeId" });
            DropIndex("dbo.UserTicketGenerations", new[] { "TicketGenerationId" });
            DropTable("dbo.UserTicketGenerations");
        }
    }
}
