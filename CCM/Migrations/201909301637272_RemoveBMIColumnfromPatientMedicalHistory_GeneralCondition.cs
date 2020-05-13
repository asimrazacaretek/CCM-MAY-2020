namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBMIColumnfromPatientMedicalHistory_GeneralCondition : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PatientMedicalHistory_GeneralCondition", "BMIType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientMedicalHistory_GeneralCondition", "BMIType", c => c.String());
        }
    }
}
