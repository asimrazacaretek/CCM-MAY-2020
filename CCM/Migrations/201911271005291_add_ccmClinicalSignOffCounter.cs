namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ccmClinicalSignOffCounter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CCMCycleStatus", "clinicalSignOffCounter", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CCMCycleStatus", "clinicalSignOffCounter");
        }
    }
}
