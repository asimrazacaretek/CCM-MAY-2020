namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnstofinalcareplan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinalCarePlanNotes", "NewWound", c => c.Boolean(nullable: false));
            AddColumn("dbo.FinalCarePlanNotes", "Bleedinggums", c => c.Boolean(nullable: false));
            AddColumn("dbo.FinalCarePlanNotes", "Nosebleed", c => c.Boolean(nullable: false));
            AddColumn("dbo.FinalCarePlanNotes", "Bloodinurine", c => c.Boolean(nullable: false));
            AddColumn("dbo.FinalCarePlanNotes", "Isurinemalodorous", c => c.Boolean(nullable: false));
            AddColumn("dbo.FinalCarePlanNotes", "Suddenincreasedswelling", c => c.Boolean(nullable: false));
            AddColumn("dbo.FinalCarePlanNotes", "SkinAssesment", c => c.String(maxLength: 5, unicode: false));
            AddColumn("dbo.FinalCarePlanNotes", "Diaperchange", c => c.String(maxLength: 5, unicode: false));
            AddColumn("dbo.FinalCarePlanNotes", "DMEsupply", c => c.String());
            AddColumn("dbo.FinalCarePlanNotes", "isPatientBedbound", c => c.String(maxLength: 5, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinalCarePlanNotes", "isPatientBedbound");
            DropColumn("dbo.FinalCarePlanNotes", "DMEsupply");
            DropColumn("dbo.FinalCarePlanNotes", "Diaperchange");
            DropColumn("dbo.FinalCarePlanNotes", "SkinAssesment");
            DropColumn("dbo.FinalCarePlanNotes", "Suddenincreasedswelling");
            DropColumn("dbo.FinalCarePlanNotes", "Isurinemalodorous");
            DropColumn("dbo.FinalCarePlanNotes", "Bloodinurine");
            DropColumn("dbo.FinalCarePlanNotes", "Nosebleed");
            DropColumn("dbo.FinalCarePlanNotes", "Bleedinggums");
            DropColumn("dbo.FinalCarePlanNotes", "NewWound");
        }
    }
}
