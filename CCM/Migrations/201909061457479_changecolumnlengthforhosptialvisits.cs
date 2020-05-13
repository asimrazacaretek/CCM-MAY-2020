namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnlengthforhosptialvisits : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientProfile_Hospitalvisits", "ICD10Codes", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientProfile_Hospitalvisits", "ICD10Codes", c => c.String(maxLength: 250, unicode: false));
        }
    }
}
