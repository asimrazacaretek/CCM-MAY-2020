namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnsPatientMedicalHistory_MedicationRx_DiscontinueMedicine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientMedicalHistory_MedicationRx", "IsPermanentMedicine", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientMedicalHistory_MedicationRx", "IsMedicineDiscontinued", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientMedicalHistory_MedicationRx", "IsMedicineDiscontinued");
            DropColumn("dbo.PatientMedicalHistory_MedicationRx", "IsPermanentMedicine");
        }
    }
}
