namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addHospitalDepartmentstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HospitalDepartments",
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
            
            AddColumn("dbo.PatientProfile_Hospitalvisits", "HospitalDepartmentsId", c => c.Int());
            CreateIndex("dbo.PatientProfile_Hospitalvisits", "HospitalDepartmentsId");
            AddForeignKey("dbo.PatientProfile_Hospitalvisits", "HospitalDepartmentsId", "dbo.HospitalDepartments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientProfile_Hospitalvisits", "HospitalDepartmentsId", "dbo.HospitalDepartments");
            DropIndex("dbo.PatientProfile_Hospitalvisits", new[] { "HospitalDepartmentsId" });
            DropColumn("dbo.PatientProfile_Hospitalvisits", "HospitalDepartmentsId");
            DropTable("dbo.HospitalDepartments");
        }
    }
}
