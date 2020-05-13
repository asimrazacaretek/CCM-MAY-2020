namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newTablePatient_LaboratoryResult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patient_LaboratoryResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        Files = c.Binary(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Patient_LaboratoryResult");
        }
    }
}
