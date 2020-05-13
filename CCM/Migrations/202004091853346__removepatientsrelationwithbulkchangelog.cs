namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _removepatientsrelationwithbulkchangelog : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BulkChangesLogs", "PatientId", "dbo.Patients");
            DropIndex("dbo.BulkChangesLogs", new[] { "PatientId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.BulkChangesLogs", "PatientId");
            AddForeignKey("dbo.BulkChangesLogs", "PatientId", "dbo.Patients", "Id");
        }
    }
}
