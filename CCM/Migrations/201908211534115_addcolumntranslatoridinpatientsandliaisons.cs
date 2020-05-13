namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumntranslatoridinpatientsandliaisons : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Liaisons", "IsTranslator", c => c.Boolean(nullable: false));
            AddColumn("dbo.Patients", "TranslatorId", c => c.Int());
            AddColumn("dbo.Patients", "TranslatorAssignedOn", c => c.DateTime());
            AddColumn("dbo.Patients", "TranslatorAssignedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "TranslatorAssignedBy");
            DropColumn("dbo.Patients", "TranslatorAssignedOn");
            DropColumn("dbo.Patients", "TranslatorId");
            DropColumn("dbo.Liaisons", "IsTranslator");
        }
    }
}
