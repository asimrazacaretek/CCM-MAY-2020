namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnAdditionalProvidersInDoctorVisit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorVisits", "AdditionalProviders", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorVisits", "AdditionalProviders");
        }
    }
}
