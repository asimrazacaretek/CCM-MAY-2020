namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsignaturetopatientandpatientprofileconsent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "Signature", c => c.Binary());
            AddColumn("dbo.PatientProfile_Consent", "Signature", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientProfile_Consent", "Signature");
            DropColumn("dbo.Patients", "Signature");
        }
    }
}
