namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _EvaluationFormDatabaseStructureDesign : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Evaluation_MainQuestionAnswer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(),
                        Question = c.String(),
                        MainQuestionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evaluation_Questions", t => t.MainQuestionId)
                .ForeignKey("dbo.QuestionBanks", t => t.QuestionId)
                .Index(t => t.QuestionId)
                .Index(t => t.MainQuestionId);
            
            CreateTable(
                "dbo.Evaluation_SubQuestionAnswer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(),
                        Question = c.String(),
                        Type = c.String(),
                        HaveDateTime = c.Boolean(nullable: false),
                        SubQuestionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evaluation_SubQuestions", t => t.SubQuestionId)
                .ForeignKey("dbo.QuestionBanks", t => t.QuestionId)
                .Index(t => t.QuestionId)
                .Index(t => t.SubQuestionId);
            
            AddColumn("dbo.Evaluation_Questions", "HasDate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Evaluation_SubQuestions", "HasDate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Evaluation_SubQuestionAnswer", "QuestionId", "dbo.QuestionBanks");
            DropForeignKey("dbo.Evaluation_SubQuestionAnswer", "SubQuestionId", "dbo.Evaluation_SubQuestions");
            DropForeignKey("dbo.Evaluation_MainQuestionAnswer", "QuestionId", "dbo.QuestionBanks");
            DropForeignKey("dbo.Evaluation_MainQuestionAnswer", "MainQuestionId", "dbo.Evaluation_Questions");
            DropIndex("dbo.Evaluation_SubQuestionAnswer", new[] { "SubQuestionId" });
            DropIndex("dbo.Evaluation_SubQuestionAnswer", new[] { "QuestionId" });
            DropIndex("dbo.Evaluation_MainQuestionAnswer", new[] { "MainQuestionId" });
            DropIndex("dbo.Evaluation_MainQuestionAnswer", new[] { "QuestionId" });
            DropColumn("dbo.Evaluation_SubQuestions", "HasDate");
            DropColumn("dbo.Evaluation_Questions", "HasDate");
            DropTable("dbo.Evaluation_SubQuestionAnswer");
            DropTable("dbo.Evaluation_MainQuestionAnswer");
        }
    }
}
