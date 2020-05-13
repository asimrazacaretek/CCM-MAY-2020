namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _EvaluationFormDatabaseStructureDesignChanges2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evaluation_MainQuestionAnswer", "Status", c => c.Boolean(nullable: false));
            AddColumn("dbo.Evaluation_MainQuestionAnswer", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.Evaluation_MainQuestionAnswer", "CreatedBy", c => c.String());
            AddColumn("dbo.Evaluation_MainQuestionAnswer", "UpdatedOn", c => c.DateTime());
            AddColumn("dbo.Evaluation_MainQuestionAnswer", "UpdatedBy", c => c.String());
            AddColumn("dbo.Evaluation_SubQuestions", "Type", c => c.String());
            AddColumn("dbo.Evaluation_SubQuestionAnswer", "Status", c => c.Boolean(nullable: false));
            AddColumn("dbo.Evaluation_SubQuestionAnswer", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.Evaluation_SubQuestionAnswer", "CreatedBy", c => c.String());
            AddColumn("dbo.Evaluation_SubQuestionAnswer", "UpdatedOn", c => c.DateTime());
            AddColumn("dbo.Evaluation_SubQuestionAnswer", "UpdatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Evaluation_SubQuestionAnswer", "UpdatedBy");
            DropColumn("dbo.Evaluation_SubQuestionAnswer", "UpdatedOn");
            DropColumn("dbo.Evaluation_SubQuestionAnswer", "CreatedBy");
            DropColumn("dbo.Evaluation_SubQuestionAnswer", "CreatedOn");
            DropColumn("dbo.Evaluation_SubQuestionAnswer", "Status");
            DropColumn("dbo.Evaluation_SubQuestions", "Type");
            DropColumn("dbo.Evaluation_MainQuestionAnswer", "UpdatedBy");
            DropColumn("dbo.Evaluation_MainQuestionAnswer", "UpdatedOn");
            DropColumn("dbo.Evaluation_MainQuestionAnswer", "CreatedBy");
            DropColumn("dbo.Evaluation_MainQuestionAnswer", "CreatedOn");
            DropColumn("dbo.Evaluation_MainQuestionAnswer", "Status");
        }
    }
}
