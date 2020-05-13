namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addtableHospitalProcedures : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HospitalProcedures",
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
            
            AddColumn("dbo.PatientProfile_Hospitalvisits", "HospitalProceduresId", c => c.Int());
            CreateIndex("dbo.PatientProfile_Hospitalvisits", "HospitalProceduresId");
            AddForeignKey("dbo.PatientProfile_Hospitalvisits", "HospitalProceduresId", "dbo.HospitalProcedures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientProfile_Hospitalvisits", "HospitalProceduresId", "dbo.HospitalProcedures");
            DropIndex("dbo.PatientProfile_Hospitalvisits", new[] { "HospitalProceduresId" });
            DropColumn("dbo.PatientProfile_Hospitalvisits", "HospitalProceduresId");
            DropTable("dbo.HospitalProcedures");
        }
    }
}
