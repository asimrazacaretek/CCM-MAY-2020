namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class G0506_tables_Plus_EvaluationFormDataSave : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EvaluationFormDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MainQuestionGuid = c.String(),
                        AnswerGuid = c.String(),
                        Answer = c.String(),
                        Status = c.Boolean(nullable: false),
                        HasDate = c.Boolean(nullable: false),
                        Date = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EvaluationFormDataAnswersForCheckBoxes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnswerGuid = c.String(),
                        Answer = c.String(),
                        Status = c.Boolean(nullable: false),
                        HasDate = c.Boolean(nullable: false),
                        Date = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        EvaluationFormDataId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EvaluationFormDatas", t => t.EvaluationFormDataId)
                .Index(t => t.EvaluationFormDataId);
            
            CreateTable(
                "dbo.G0506_PrimaryInsurance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PlanName = c.String(),
                        InsuranceId = c.Int(),
                        GroupNumber = c.Int(),
                        PlanCode = c.String(),
                        RxBin = c.String(),
                        RxPCN = c.String(),
                        RxGroup = c.String(),
                        RelatedtoInsurance = c.String(),
                        Status = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.G0506_SecondaryInsurance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MedicareNo = c.Int(),
                        MedicaidNo = c.Int(),
                        OtherInsuranceNo = c.Int(),
                        Status = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.G0506_PatientsInfo", "G0506_PrimaryInsuranceId", c => c.Int());
            AddColumn("dbo.G0506_PatientsInfo", "G0506_SecondaryInsuranceId", c => c.Int());
            CreateIndex("dbo.G0506_PatientsInfo", "G0506_PrimaryInsuranceId");
            CreateIndex("dbo.G0506_PatientsInfo", "G0506_SecondaryInsuranceId");
            AddForeignKey("dbo.G0506_PatientsInfo", "G0506_PrimaryInsuranceId", "dbo.G0506_PrimaryInsurance", "Id");
            AddForeignKey("dbo.G0506_PatientsInfo", "G0506_SecondaryInsuranceId", "dbo.G0506_SecondaryInsurance", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.G0506_PatientsInfo", "G0506_SecondaryInsuranceId", "dbo.G0506_SecondaryInsurance");
            DropForeignKey("dbo.G0506_PatientsInfo", "G0506_PrimaryInsuranceId", "dbo.G0506_PrimaryInsurance");
            DropForeignKey("dbo.EvaluationFormDataAnswersForCheckBoxes", "EvaluationFormDataId", "dbo.EvaluationFormDatas");
            DropIndex("dbo.G0506_PatientsInfo", new[] { "G0506_SecondaryInsuranceId" });
            DropIndex("dbo.G0506_PatientsInfo", new[] { "G0506_PrimaryInsuranceId" });
            DropIndex("dbo.EvaluationFormDataAnswersForCheckBoxes", new[] { "EvaluationFormDataId" });
            DropColumn("dbo.G0506_PatientsInfo", "G0506_SecondaryInsuranceId");
            DropColumn("dbo.G0506_PatientsInfo", "G0506_PrimaryInsuranceId");
            DropTable("dbo.G0506_SecondaryInsurance");
            DropTable("dbo.G0506_PrimaryInsurance");
            DropTable("dbo.EvaluationFormDataAnswersForCheckBoxes");
            DropTable("dbo.EvaluationFormDatas");
        }
    }
}
