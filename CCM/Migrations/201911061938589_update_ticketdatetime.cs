namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_ticketdatetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TicketGenerations", "updatedDate", c => c.DateTime());
            AlterColumn("dbo.TicketGenerations", "deletedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TicketGenerations", "deletedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TicketGenerations", "updatedDate", c => c.DateTime(nullable: false));
        }
    }
}
