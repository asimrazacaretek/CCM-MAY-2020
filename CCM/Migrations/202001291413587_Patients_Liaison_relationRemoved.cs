namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patients_Liaison_relationRemoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Patients_Liaisons", "LiaisonId", "dbo.Liaisons");
            DropForeignKey("dbo.Patients_Liaisons", "PatientId", "dbo.Patients");
            DropIndex("dbo.Patients_Liaisons", new[] { "PatientId" });
            DropIndex("dbo.Patients_Liaisons", new[] { "LiaisonId" });
            DropTable("dbo.Patients_Liaisons");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Patients_Liaisons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(),
                        LiaisonId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Patients_Liaisons", "LiaisonId");
            CreateIndex("dbo.Patients_Liaisons", "PatientId");
            AddForeignKey("dbo.Patients_Liaisons", "PatientId", "dbo.Patients", "Id");
            AddForeignKey("dbo.Patients_Liaisons", "LiaisonId", "dbo.Liaisons", "Id");
        }
    }
}
