namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_UserTicketGenerationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTicketGenerations", "creatorNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTicketGenerations", "creatorNotes");
        }
    }
}
