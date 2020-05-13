namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnpicassocheckedandnewtablemedicareandmedicaideligibilty : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientMeidcareMedicaidEligibilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        MedicareEligibilty = c.String(maxLength: 5, unicode: false),
                        MedicareEligibiltyNotes = c.String(),
                        MedicareEligibiltySceenshot = c.Binary(),
                        MedicaidEligibilty = c.String(maxLength: 5, unicode: false),
                        MedicaidEligibiltyNotes = c.String(),
                        MedicaidEligibiltySceenshot = c.Binary(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Patients", "PicassoChecked", c => c.String(maxLength: 5, unicode: false));
            AddColumn("dbo.Patients", "PatientMeidcareMedicaidEligibility_Id", c => c.Int());
            AlterColumn("dbo.Patients", "EMRNumber", c => c.String(nullable: false, maxLength: 100, unicode: false));
            CreateIndex("dbo.Patients", "PatientMeidcareMedicaidEligibility_Id");
            AddForeignKey("dbo.Patients", "PatientMeidcareMedicaidEligibility_Id", "dbo.PatientMeidcareMedicaidEligibilities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "PatientMeidcareMedicaidEligibility_Id", "dbo.PatientMeidcareMedicaidEligibilities");
            DropIndex("dbo.Patients", new[] { "PatientMeidcareMedicaidEligibility_Id" });
            AlterColumn("dbo.Patients", "EMRNumber", c => c.String(maxLength: 100, unicode: false));
            DropColumn("dbo.Patients", "PatientMeidcareMedicaidEligibility_Id");
            DropColumn("dbo.Patients", "PicassoChecked");
            DropTable("dbo.PatientMeidcareMedicaidEligibilities");
        }
    }
}
