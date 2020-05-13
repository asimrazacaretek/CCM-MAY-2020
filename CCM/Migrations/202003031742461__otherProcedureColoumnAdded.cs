namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _otherProcedureColoumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientProfile_Hospitalvisits", "OtherProcedure", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientProfile_Hospitalvisits", "OtherProcedure");
        }
    }
}
