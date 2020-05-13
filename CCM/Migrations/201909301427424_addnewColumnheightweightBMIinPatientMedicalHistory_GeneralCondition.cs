namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewColumnheightweightBMIinPatientMedicalHistory_GeneralCondition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientMedicalHistory_GeneralCondition", "BMIType", c => c.String());
            AddColumn("dbo.PatientMedicalHistory_GeneralCondition", "Height", c => c.Double(nullable: false));
            AddColumn("dbo.PatientMedicalHistory_GeneralCondition", "Weight", c => c.Double(nullable: false));
            AddColumn("dbo.PatientMedicalHistory_GeneralCondition", "BMI", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientMedicalHistory_GeneralCondition", "BMI");
            DropColumn("dbo.PatientMedicalHistory_GeneralCondition", "Weight");
            DropColumn("dbo.PatientMedicalHistory_GeneralCondition", "Height");
            DropColumn("dbo.PatientMedicalHistory_GeneralCondition", "BMIType");
        }
    }
}
