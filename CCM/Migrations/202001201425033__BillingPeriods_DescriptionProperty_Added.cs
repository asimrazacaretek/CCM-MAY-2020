namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _BillingPeriods_DescriptionProperty_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingPeriods", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillingPeriods", "Description");
        }
    }
}
