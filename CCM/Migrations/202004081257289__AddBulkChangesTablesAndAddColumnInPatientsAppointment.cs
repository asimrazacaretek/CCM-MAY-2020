namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddBulkChangesTablesAndAddColumnInPatientsAppointment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BulkChanges",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.BulkChangesLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(),
                        ResultMessage = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Createdby = c.String(),
                        BulkChangeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BulkChanges", t => t.BulkChangeId)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.BulkChangeId);
            
            AddColumn("dbo.PatientAppointments", "BillingCategoryId", c => c.Int());
            CreateIndex("dbo.PatientAppointments", "BillingCategoryId");
            AddForeignKey("dbo.PatientAppointments", "BillingCategoryId", "dbo.BillingCategories", "BillingCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientAppointments", "BillingCategoryId", "dbo.BillingCategories");
            DropForeignKey("dbo.BulkChangesLogs", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.BulkChangesLogs", "BulkChangeId", "dbo.BulkChanges");
            DropIndex("dbo.PatientAppointments", new[] { "BillingCategoryId" });
            DropIndex("dbo.BulkChangesLogs", new[] { "BulkChangeId" });
            DropIndex("dbo.BulkChangesLogs", new[] { "PatientId" });
            DropColumn("dbo.PatientAppointments", "BillingCategoryId");
            DropTable("dbo.BulkChangesLogs");
            DropTable("dbo.BulkChanges");
        }
    }
}
