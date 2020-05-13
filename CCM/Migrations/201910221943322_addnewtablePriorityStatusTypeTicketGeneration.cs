namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablePriorityStatusTypeTicketGeneration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Priorities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        priorityLevel = c.String(),
                        priorityMinute = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        statusName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TicketGenerations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        subjectName = c.String(),
                        TypeId = c.Int(),
                        PriorityId = c.Int(),
                        StatusId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Priorities", t => t.PriorityId)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.Types", t => t.TypeId)
                .Index(t => t.TypeId)
                .Index(t => t.PriorityId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        typeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketGenerations", "TypeId", "dbo.Types");
            DropForeignKey("dbo.TicketGenerations", "StatusId", "dbo.Status");
            DropForeignKey("dbo.TicketGenerations", "PriorityId", "dbo.Priorities");
            DropIndex("dbo.TicketGenerations", new[] { "StatusId" });
            DropIndex("dbo.TicketGenerations", new[] { "PriorityId" });
            DropIndex("dbo.TicketGenerations", new[] { "TypeId" });
            DropTable("dbo.Types");
            DropTable("dbo.TicketGenerations");
            DropTable("dbo.Status");
            DropTable("dbo.Priorities");
        }
    }
}
