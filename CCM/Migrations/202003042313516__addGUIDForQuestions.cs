namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addGUIDForQuestions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evaluation_MainQuestionAnswer", "QuestionGUID", c => c.String());
            AddColumn("dbo.Evaluation_Questions", "QuestionGUID", c => c.String());
            AddColumn("dbo.Evaluation_SubQuestions", "QuestionGUID", c => c.String());
            AddColumn("dbo.Evaluation_SubQuestionAnswer", "QuestionGUID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Evaluation_SubQuestionAnswer", "QuestionGUID");
            DropColumn("dbo.Evaluation_SubQuestions", "QuestionGUID");
            DropColumn("dbo.Evaluation_Questions", "QuestionGUID");
            DropColumn("dbo.Evaluation_MainQuestionAnswer", "QuestionGUID");
        }
    }
}
