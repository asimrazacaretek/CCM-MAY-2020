namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnsPatientMedicalHistory_MedicationRx1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientMedicalHistory_MedicationRx", "IsTakenMedicineProperly", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientMedicalHistory_MedicationRx", "TakenMedicineProperly", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientMedicalHistory_MedicationRx", "TakenMedicineProperly");
            DropColumn("dbo.PatientMedicalHistory_MedicationRx", "IsTakenMedicineProperly");
        }
    }
}
