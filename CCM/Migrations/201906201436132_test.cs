namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        Module = c.String(maxLength: 100),
                        PatientId = c.Int(nullable: false),
                        isToShowinPopup = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CallHistories", "Direction", c => c.String());
            AddColumn("dbo.Icd10Codes", "DiseaseState", c => c.String());
            AddColumn("dbo.Icd10Codes", "DiseaseType", c => c.String());
            AddColumn("dbo.Icd10Codes", "DiseaseHistory", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Icd10Codes", "DiseaseHistory");
            DropColumn("dbo.Icd10Codes", "DiseaseType");
            DropColumn("dbo.Icd10Codes", "DiseaseState");
            DropColumn("dbo.CallHistories", "Direction");
            DropTable("dbo.PatientNotes");
        }
    }
}
