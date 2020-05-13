namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumntype2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinalCarePlanNotes", "PatientSummary", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FinalCarePlanNotes", "PatientSummary", c => c.String(nullable: false));
        }
    }
}
