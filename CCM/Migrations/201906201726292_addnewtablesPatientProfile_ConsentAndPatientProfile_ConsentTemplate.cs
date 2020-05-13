namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablesPatientProfile_ConsentAndPatientProfile_ConsentTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientProfile_Consent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        Consent = c.String(maxLength: 250, unicode: false),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientProfile_ConsentTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConsentTemplate = c.String(maxLength: 250, unicode: false),
                        Type = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PatientProfile_ConsentTemplate");
            DropTable("dbo.PatientProfile_Consent");
        }
    }
}
