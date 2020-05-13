namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtableMedicationDoseRepitionTimes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedicationDoseRepetitionTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoseRepetitionTime = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MedicationDoseRepetitionTimes");
        }
    }
}
