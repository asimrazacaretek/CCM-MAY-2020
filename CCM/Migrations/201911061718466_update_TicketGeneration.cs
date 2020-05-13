namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_TicketGeneration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketGenerations", "createdDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.TicketGenerations", "createdBy", c => c.String());
            AddColumn("dbo.TicketGenerations", "updatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.TicketGenerations", "updateBy", c => c.String());
            AddColumn("dbo.TicketGenerations", "isDeleted", c => c.Boolean());
            AddColumn("dbo.TicketGenerations", "deletedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketGenerations", "deletedDate");
            DropColumn("dbo.TicketGenerations", "isDeleted");
            DropColumn("dbo.TicketGenerations", "updateBy");
            DropColumn("dbo.TicketGenerations", "updatedDate");
            DropColumn("dbo.TicketGenerations", "createdBy");
            DropColumn("dbo.TicketGenerations", "createdDate");
        }
    }
}
