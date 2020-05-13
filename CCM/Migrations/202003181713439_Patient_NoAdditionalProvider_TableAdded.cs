namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patient_NoAdditionalProvider_TableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patient_NoAdditionalProvider",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PatientId = c.Int(),
                        PatientNoSecondaryDoctor = c.Boolean(nullable: false),
                        Status = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Patient_NoAdditionalProvider");
        }
    }
}
