namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewcolumnsInPatientLifestyle_WorkAndRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCA", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCAHrsweek", c => c.String());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAID", c => c.Int());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAHrsweek", c => c.String());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NID", c => c.Int());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NHrsweek", c => c.String());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "TID", c => c.Int());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "THrsweek", c => c.String());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwID", c => c.Int());
            AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwHrsweek", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwHrsweek");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwID");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "THrsweek");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "TID");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NHrsweek");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NID");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAHrsweek");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAID");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCAHrsweek");
            DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCA");
        }
    }
}
