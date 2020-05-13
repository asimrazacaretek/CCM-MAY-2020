namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumntypedateinmedication : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "StartDate", c => c.DateTime());
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PatientMedicalHistory_MedicationRx", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
