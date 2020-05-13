namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patient_PatientsPreLiasons_relationShip : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients_PreLiaisons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LiaisonId = c.Int(),
                        TranslatorId = c.Int(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Patients", "Patients_PreLiaisonsId", c => c.Int());
            CreateIndex("dbo.Patients", "Patients_PreLiaisonsId");
            AddForeignKey("dbo.Patients", "Patients_PreLiaisonsId", "dbo.Patients_PreLiaisons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "Patients_PreLiaisonsId", "dbo.Patients_PreLiaisons");
            DropIndex("dbo.Patients", new[] { "Patients_PreLiaisonsId" });
            DropColumn("dbo.Patients", "Patients_PreLiaisonsId");
            DropTable("dbo.Patients_PreLiaisons");
        }
    }
}
