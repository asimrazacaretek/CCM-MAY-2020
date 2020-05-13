namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTableLabResultToPatientImages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patient_Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 8000, unicode: false),
                        FilePath = c.String(maxLength: 8000, unicode: false),
                        ImgType = c.String(maxLength: 8000, unicode: false),
                        MimeType = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Patient_LaboratoryResult");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Patient_LaboratoryResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        Files = c.String(maxLength: 8000, unicode: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Patient_Images");
        }
    }
}
