namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnstoliaisonfortwilio : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Liaisons", "TwilioAccountSID", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Liaisons", "TwilioAuthToken", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Liaisons", "TwilioCallerId", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.Liaisons", "TwilioTwimlAppSid", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Liaisons", "TwilioTwimlAppSid");
            DropColumn("dbo.Liaisons", "TwilioCallerId");
            DropColumn("dbo.Liaisons", "TwilioAuthToken");
            DropColumn("dbo.Liaisons", "TwilioAccountSID");
        }
    }
}
