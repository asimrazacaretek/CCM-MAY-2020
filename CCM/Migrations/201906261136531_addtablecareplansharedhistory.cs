namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablecareplansharedhistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CarePlanSharedHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        PatientId = c.Int(nullable: false),
                        Cycle = c.Int(nullable: false),
                        EmailSentTo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CarePlanSharedHistories");
        }
    }
}
