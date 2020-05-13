namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _final : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCA");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCAHrsweek");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAID");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAHrsweek");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NID");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "NHrsweek");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "TID");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "THrsweek");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwID");
            //DropColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwHrsweek");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwHrsweek", c => c.Double());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "SwID", c => c.Int());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "THrsweek", c => c.Double());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "TID", c => c.Int());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NHrsweek", c => c.Double());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NID", c => c.Int());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAHrsweek", c => c.Double());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "CAID", c => c.Int());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCAHrsweek", c => c.Double());
            //AddColumn("dbo.PatientLifestyle_WorkAndRelationship", "NCA", c => c.Boolean(nullable: false));
        }
    }
}
