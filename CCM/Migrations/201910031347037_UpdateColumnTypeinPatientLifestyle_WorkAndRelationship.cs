namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateColumnTypeinPatientLifestyle_WorkAndRelationship : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCAHrsweek", c => c.Double());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAHrsweek", c => c.Double());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "NHrsweek", c => c.Double());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "THrsweek", c => c.Double());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwHrsweek", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwHrsweek", c => c.String());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "THrsweek", c => c.String());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "NHrsweek", c => c.String());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAHrsweek", c => c.String());
            AlterColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCAHrsweek", c => c.String());
        }
    }
}
