namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumntypePatientMedicalHistory_MedicationRx : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "StartDate", c => c.DateTime(nullable: true));
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "EndDate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "EndDate", c => c.String());
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "StartDate", c => c.String());
        }
    }
}
