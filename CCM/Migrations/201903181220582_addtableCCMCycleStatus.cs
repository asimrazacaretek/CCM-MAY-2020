namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtableCCMCycleStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CCMCycleStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        CCMStatus = c.String(maxLength: 50, unicode: false),
                        CCMSubStatus = c.String(maxLength: 50, unicode: false),
                        Cycle = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CCMCycleStatus", "PatientId", "dbo.Patients");
            DropIndex("dbo.CCMCycleStatus", new[] { "PatientId" });
            DropTable("dbo.CCMCycleStatus");
        }
    }
}
