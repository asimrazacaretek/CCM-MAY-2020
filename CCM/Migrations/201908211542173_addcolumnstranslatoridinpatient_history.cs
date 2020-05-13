namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnstranslatoridinpatient_history : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients_History", "TranslatorId", c => c.Int());
            AddColumn("dbo.Patients_History", "TranslatorAssignedOn", c => c.DateTime());
            AddColumn("dbo.Patients_History", "TranslatorAssignedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients_History", "TranslatorAssignedBy");
            DropColumn("dbo.Patients_History", "TranslatorAssignedOn");
            DropColumn("dbo.Patients_History", "TranslatorId");
        }
    }
}
