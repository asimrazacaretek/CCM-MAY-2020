namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _PatientProfile_Hospitalvisits_History_tableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientProfile_Hospitalvisits_History",
                c => new
                {
                    Id = c.Int(),
                    PatientId = c.Int(),
                    HospitalName = c.String(),
                    HNPI = c.String(),
                    HType = c.String(),
                    HAddress = c.String(),
                    AdmitDate = c.DateTime(nullable: false),
                    DischargeDate = c.DateTime(nullable: false),
                    Rate = c.Int(nullable: false),
                    TotalDays = c.Int(nullable: false),
                    Reason = c.String(),
                    Department = c.String(),
                    ICD10Codes = c.String(),
                    FullName = c.String(),
                    NPI = c.String(),
                    Phone = c.String(),
                    StayType = c.String(),
                    CreatedBy = c.String(),
                    CreatedOn = c.DateTime(nullable: false),
                    UpdatedBy = c.String(),
                    UpdatedOn = c.DateTime(nullable: false),
                    HospitalReasonsId = c.Int(),
                    HospitalDepartmentsId = c.Int(),
                    HospitalProceduresId = c.Int(),
                });
              
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PatientProfile_Hospitalvisits_History");
        }
    }
}
