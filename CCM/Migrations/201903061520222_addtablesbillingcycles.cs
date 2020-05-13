namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablesbillingcycles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillingCycleDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillingCycleId = c.Int(nullable: false),
                        RecordingID = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        isDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCycles", t => t.BillingCycleId, cascadeDelete: true)
                .Index(t => t.BillingCycleId);
            
            CreateTable(
                "dbo.BillingCycles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        Cycle = c.Int(nullable: false),
                        BillingCode1 = c.String(maxLength: 50, unicode: false),
                        BillingCode2 = c.String(maxLength: 50, unicode: false),
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
            DropForeignKey("dbo.BillingCycles", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.BillingCycleDetails", "BillingCycleId", "dbo.BillingCycles");
            DropIndex("dbo.BillingCycles", new[] { "PatientId" });
            DropIndex("dbo.BillingCycleDetails", new[] { "BillingCycleId" });
            DropTable("dbo.BillingCycles");
            DropTable("dbo.BillingCycleDetails");
        }
    }
}
