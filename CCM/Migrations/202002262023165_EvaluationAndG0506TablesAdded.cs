namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvaluationAndG0506TablesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Evaluations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        BillingCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCategories", t => t.BillingCategoryId)
                .Index(t => t.BillingCategoryId);
            
            CreateTable(
                "dbo.Evaluation_Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(),
                        HasSubtype = c.Boolean(nullable: false),
                        Type = c.String(),
                        Order = c.Int(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        EvaluationsId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evaluations", t => t.EvaluationsId)
                .ForeignKey("dbo.QuestionBanks", t => t.QuestionId)
                .Index(t => t.QuestionId)
                .Index(t => t.EvaluationsId);
            
            CreateTable(
                "dbo.Evaluation_SubQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        QuestionId = c.Int(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        Evaluation_QuestionsId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evaluation_Questions", t => t.Evaluation_QuestionsId)
                .ForeignKey("dbo.QuestionBanks", t => t.QuestionId)
                .Index(t => t.QuestionId)
                .Index(t => t.Evaluation_QuestionsId);
            
            CreateTable(
                "dbo.G0506_AdditionalProviders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Speciality = c.String(),
                        LastVisit = c.String(),
                        NextAppointment = c.String(),
                        DoctorType = c.String(),
                        isCCMProvider = c.Boolean(nullable: false),
                        MobilePhoneNumber = c.String(),
                        Email = c.String(),
                        IsShareCarePlan = c.Boolean(),
                        NPI = c.Int(),
                        MainPhoneNumber = c.String(nullable: false),
                        Address1 = c.String(maxLength: 100, unicode: false),
                        Address2 = c.String(maxLength: 100, unicode: false),
                        IsPrimaryCareProvider = c.Boolean(nullable: false),
                        G0506_PatientsInfoId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.G0506_PatientsInfo", t => t.G0506_PatientsInfoId)
                .Index(t => t.G0506_PatientsInfoId);
            
            CreateTable(
                "dbo.G0506_PatientsInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(),
                        FullName = c.String(),
                        BirthDate = c.DateTime(),
                        IsCurrentlyActiveinCCM = c.Boolean(),
                        IsCCMConsentcompleted = c.Boolean(),
                        DateConsentcompleted = c.DateTime(),
                        DesignatedCCMContact = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.G0506_AdditionalProviders", "G0506_PatientsInfoId", "dbo.G0506_PatientsInfo");
            DropForeignKey("dbo.G0506_PatientsInfo", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Evaluation_Questions", "QuestionId", "dbo.QuestionBanks");
            DropForeignKey("dbo.Evaluation_Questions", "EvaluationsId", "dbo.Evaluations");
            DropForeignKey("dbo.Evaluation_SubQuestions", "QuestionId", "dbo.QuestionBanks");
            DropForeignKey("dbo.Evaluation_SubQuestions", "Evaluation_QuestionsId", "dbo.Evaluation_Questions");
            DropForeignKey("dbo.Evaluations", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.G0506_PatientsInfo", new[] { "PatientId" });
            DropIndex("dbo.G0506_AdditionalProviders", new[] { "G0506_PatientsInfoId" });
            DropIndex("dbo.Evaluation_SubQuestions", new[] { "Evaluation_QuestionsId" });
            DropIndex("dbo.Evaluation_SubQuestions", new[] { "QuestionId" });
            DropIndex("dbo.Evaluation_Questions", new[] { "EvaluationsId" });
            DropIndex("dbo.Evaluation_Questions", new[] { "QuestionId" });
            DropIndex("dbo.Evaluations", new[] { "BillingCategoryId" });
            DropTable("dbo.G0506_PatientsInfo");
            DropTable("dbo.G0506_AdditionalProviders");
            DropTable("dbo.Evaluation_SubQuestions");
            DropTable("dbo.Evaluation_Questions");
            DropTable("dbo.Evaluations");
        }
    }
}
