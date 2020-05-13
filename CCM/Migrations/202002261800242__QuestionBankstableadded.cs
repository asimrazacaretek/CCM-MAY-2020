namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _QuestionBankstableadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionBanks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Question = c.String(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QuestionBanks");
        }
    }
}
