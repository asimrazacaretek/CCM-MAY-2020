namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class G0506_tables_Plus_EvaluationFormDataSave__Change2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EvaluationFormDatas", "PatientId", c => c.Int());
            AddColumn("dbo.EvaluationFormDatas", "EvaluationFormId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EvaluationFormDatas", "EvaluationFormId");
            DropColumn("dbo.EvaluationFormDatas", "PatientId");
        }
    }
}
