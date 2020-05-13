namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketHonorIdANDisTicketHonorINUserTicketGeneration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTicketGenerations", "ticketHonorId", c => c.String());
            AddColumn("dbo.UserTicketGenerations", "isTicketHonor", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTicketGenerations", "isTicketHonor");
            DropColumn("dbo.UserTicketGenerations", "ticketHonorId");
        }
    }
}
