namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class patientprofilenewtableadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientProfile_Hospitalvisits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        HospitalName = c.String(maxLength: 250, unicode: false),
                        HNPI = c.String(maxLength: 250, unicode: false),
                        HType = c.String(maxLength: 250, unicode: false),
                        HAddress = c.String(maxLength: 250, unicode: false),
                        AdmitDate = c.DateTime(nullable: false),
                        DischargeDate = c.DateTime(nullable: false),
                        Rate = c.Int(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        Reason = c.String(maxLength: 250, unicode: false),
                        Department = c.String(maxLength: 250, unicode: false),
                        ICD10Codes = c.String(maxLength: 250, unicode: false),
                        FullName = c.String(maxLength: 250, unicode: false),
                        NPI = c.String(maxLength: 100, unicode: false),
                        Phone = c.String(maxLength: 50, unicode: false),
                        StayType = c.String(maxLength: 50, unicode: false),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientProfile_Hospitalvisits", "PatientId", "dbo.Patients");
            DropIndex("dbo.PatientProfile_Hospitalvisits", new[] { "PatientId" });
            DropTable("dbo.PatientProfile_Hospitalvisits");
        }
    }
}
