namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumntwiliopathsidinliaisons : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Liaisons", "TwiliopathSid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Liaisons", "TwiliopathSid");
        }
    }
}
