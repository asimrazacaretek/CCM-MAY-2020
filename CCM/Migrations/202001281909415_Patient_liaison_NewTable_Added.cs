namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patient_liaison_NewTable_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients_Liaisons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(),
                        LiaisonId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Liaisons", t => t.LiaisonId)
                .ForeignKey("dbo.Patients", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.LiaisonId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients_Liaisons", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Patients_Liaisons", "LiaisonId", "dbo.Liaisons");
            DropIndex("dbo.Patients_Liaisons", new[] { "LiaisonId" });
            DropIndex("dbo.Patients_Liaisons", new[] { "PatientId" });
            DropTable("dbo.Patients_Liaisons");
        }
    }
}
