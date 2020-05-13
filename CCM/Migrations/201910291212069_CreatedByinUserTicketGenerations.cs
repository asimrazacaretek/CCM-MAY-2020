namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedByinUserTicketGenerations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTicketGenerations", "createdBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTicketGenerations", "createdBy");
        }
    }
}
