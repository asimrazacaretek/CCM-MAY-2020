namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _Patient_HospitalVisit_NotAdmitted_TableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patient_HospitalVisit_NotAdmitted",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PatientId = c.Int(),
                        PatientNotAdmitted = c.Boolean(nullable: false),
                        Status = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Patient_HospitalVisit_NotAdmitted");
        }
    }
}
