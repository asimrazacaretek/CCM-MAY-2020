namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_TicketGeneration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketGenerations", "deletedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketGenerations", "deletedBy");
        }
    }
}
