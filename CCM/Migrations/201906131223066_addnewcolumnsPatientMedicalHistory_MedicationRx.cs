namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnsPatientMedicalHistory_MedicationRx : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientMedicalHistory_MedicationRx", "DoseRepetitionTime", c => c.String());
            AddColumn("dbo.PatientMedicalHistory_MedicationRx", "PrescribeBy", c => c.String());
            AddColumn("dbo.PatientMedicalHistory_MedicationRx", "ForHowLong", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientMedicalHistory_MedicationRx", "ForHowLong");
            DropColumn("dbo.PatientMedicalHistory_MedicationRx", "PrescribeBy");
            DropColumn("dbo.PatientMedicalHistory_MedicationRx", "DoseRepetitionTime");
        }
    }
}
