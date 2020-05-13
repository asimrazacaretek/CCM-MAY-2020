namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTicketGeneration_Update_And_AddASSIGNEETICKET : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTicketGenerations", "createdDate", c => c.DateTime());
            AddColumn("dbo.UserTicketGenerations", "createdWeekDay", c => c.String());
            AddColumn("dbo.UserTicketGenerations", "notify", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTicketGenerations", "notify");
            DropColumn("dbo.UserTicketGenerations", "createdWeekDay");
            DropColumn("dbo.UserTicketGenerations", "createdDate");
        }
    }
}
