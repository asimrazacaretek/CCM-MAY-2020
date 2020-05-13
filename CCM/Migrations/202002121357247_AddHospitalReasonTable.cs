namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHospitalReasonTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HospitalReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PatientProfile_Hospitalvisits", "HospitalReasonsId", c => c.Int());
            CreateIndex("dbo.PatientProfile_Hospitalvisits", "HospitalReasonsId");
            AddForeignKey("dbo.PatientProfile_Hospitalvisits", "HospitalReasonsId", "dbo.HospitalReasons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientProfile_Hospitalvisits", "HospitalReasonsId", "dbo.HospitalReasons");
            DropIndex("dbo.PatientProfile_Hospitalvisits", new[] { "HospitalReasonsId" });
            DropColumn("dbo.PatientProfile_Hospitalvisits", "HospitalReasonsId");
            DropTable("dbo.HospitalReasons");
        }
    }
}
